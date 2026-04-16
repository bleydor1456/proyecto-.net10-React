using CrudNet10.Data;
using CrudNet10.Dtos;
using CrudNet10.Exceptions;
using CrudNet10.Helpers;
using CrudNet10.Middleware;
using System.Security.Claims;
using CrudNet10.Models;
using CrudNet10.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrudNet10.Services;

public class ClienteService : IClienteService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(AppDbContext context, ILogger<ClienteService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<ClienteResponseDto>>> GetClientesAsync(PaginationDto paginationDto)
    {
        if (paginationDto.Page <= 0)
            throw new BadRequestException("El número de página debe ser mayor a 0.");

        if (paginationDto.PageSize <= 0)
            throw new BadRequestException("El tamaño de página debe ser mayor a 0.");

        var query = _context.Clientes.AsQueryable();

        var totalRecords = await query.CountAsync();

        if (totalRecords == 0)
            throw new NotFoundException("No hay clientes registrados.");

        var clientes = await query
            .OrderBy(c => c.Id)
            .Skip((paginationDto.Page - 1) * paginationDto.PageSize)
            .Take(paginationDto.PageSize)
            .Select(c => new ClienteResponseDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Email = c.Email,
                Telefono = c.Telefono
            })
            .ToListAsync();

        var result = new PagedResult<ClienteResponseDto>
        {
            Items = clientes,
            Page = paginationDto.Page,
            PageSize = paginationDto.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling((double)totalRecords / paginationDto.PageSize)
        };

        _logger.LogInformation("Se obtuvieron {Cantidad} clientes.", clientes.Count);

        return ResponseHelper.Success(result, "Clientes obtenidos correctamente");
    }

    public async Task<ApiResponse<ClienteDetalleResponseDto>> GetClienteByIdAsync(int id)
    {
        if (id <= 0)
            throw new BadRequestException("El id del cliente debe ser mayor a 0.");

        var cliente = await _context.Clientes
            .Include(c => c.Pedidos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente == null)
            throw new NotFoundException($"No se encontró el cliente con id {id}.");

        var clienteDto = new ClienteDetalleResponseDto
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Email = cliente.Email,
            Telefono = cliente.Telefono,
            Pedidos = cliente.Pedidos.Select(p => new PedidoResponseDto
            {
                Id = p.Id,
                Descripcion = p.Descripcion,
                ClienteId = p.ClienteId
            }).ToList()
        };

        _logger.LogInformation("Se obtuvo el cliente con id {Id}.", id);

        return ResponseHelper.Success(clienteDto, "Cliente obtenido correctamente");
    }

    public async Task<ApiResponse<ClienteResponseDto>> CreateClienteAsync(CrearClienteDto dto, int userId)
    {
        var existeEmail = await _context.Clientes.AnyAsync(c => c.Email == dto.Email);

        if (existeEmail)
            throw new BadRequestException("Ya existe un cliente con ese email.");

        var cliente = new Cliente
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            Telefono = dto.Telefono,
            UsuarioId = userId
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        var clienteDto = new ClienteResponseDto
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Email = cliente.Email,
            Telefono = cliente.Telefono
        };

        _logger.LogInformation("Se creó el cliente con id {Id}.", cliente.Id);

        return ResponseHelper.Success(clienteDto, "Cliente creado correctamente");
    }

    public async Task<ApiResponseSimple> UpdateClienteAsync(int id, CrearClienteDto dto, int userId, bool esAdmin)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
            throw new NotFoundException($"No se encontró el cliente con id {id}.");

        if (!esAdmin && cliente.UsuarioId != userId)
            throw new UnauthorizedAccessException("No tienes permiso para modificar este cliente.");

        var existeOtroEmail = await _context.Clientes
            .AnyAsync(c => c.Email == dto.Email && c.Id != id);

        if (existeOtroEmail)
            throw new BadRequestException("Otro cliente ya usa ese email.");

        cliente.Nombre = dto.Nombre;
        cliente.Email = dto.Email;
        cliente.Telefono = dto.Telefono;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Se actualizó el cliente con id {Id}.", id);

        return ResponseHelper.Success("Cliente actualizado correctamente");
    }

    public async Task<ApiResponseSimple> DeleteClienteAsync(int id, int userId, bool esAdmin)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
            throw new NotFoundException($"No se encontró el cliente con id {id}.");

        if (!esAdmin && cliente.UsuarioId != userId)
            throw new UnauthorizedAccessException("No tienes permiso para eliminar este cliente.");

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Se eliminó el cliente con id {Id}.", id);

        return ResponseHelper.Success("Cliente eliminado correctamente");
    }

}