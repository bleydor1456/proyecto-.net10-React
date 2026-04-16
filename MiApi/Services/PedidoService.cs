using CrudNet10.Data;
using CrudNet10.Dtos;
using CrudNet10.Exceptions;
using CrudNet10.Helpers;
using CrudNet10.Middleware;
using CrudNet10.Models;
using CrudNet10.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrudNet10.Services;

public class PedidoService : IPedidoService
{
    private readonly AppDbContext _context;
    private readonly ILogger<PedidoService> _logger;

    public PedidoService(AppDbContext context, ILogger<PedidoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<PedidoResponseDto>>> GetPedidosAsync(PaginationDto paginationDto)
    {
        if (paginationDto.Page <= 0)
            throw new BadRequestException("El número de página debe ser mayor a 0.");

        if (paginationDto.PageSize <= 0)
            throw new BadRequestException("El tamaño de página debe ser mayor a 0.");

        var query = _context.Pedidos.AsQueryable();

        var totalRecords = await query.CountAsync();

        if (totalRecords == 0)
            throw new NotFoundException("No hay pedidos registrados.");

        var pedidos = await query
            .OrderBy(p => p.Id)
            .Skip((paginationDto.Page - 1) * paginationDto.PageSize)
            .Take(paginationDto.PageSize)
            .Select(p => new PedidoResponseDto
            {
                Id = p.Id,
                Descripcion = p.Descripcion,
                ClienteId = p.ClienteId
            })
            .ToListAsync();

        var result = new PagedResult<PedidoResponseDto>
        {
            Items = pedidos,
            Page = paginationDto.Page,
            PageSize = paginationDto.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling((double)totalRecords / paginationDto.PageSize)
        };

        _logger.LogInformation("Se obtuvieron {Cantidad} pedidos.", pedidos.Count);

        return ResponseHelper.Success(result, "Pedidos obtenidos correctamente");
    }

    public async Task<ApiResponse<PedidoResponseDto>> GetPedidoByIdAsync(int id)
    {
        if (id <= 0)
            throw new BadRequestException("El id del pedido debe ser mayor a 0.");

        var pedido = await _context.Pedidos.FindAsync(id);

        if (pedido == null)
            throw new NotFoundException($"No se encontró el pedido con id {id}.");

        var pedidoDto = new PedidoResponseDto
        {
            Id = pedido.Id,
            Descripcion = pedido.Descripcion,
            ClienteId = pedido.ClienteId
        };

        _logger.LogInformation("Se obtuvo el pedido con id {Id}.", id);

        return ResponseHelper.Success(pedidoDto, "Pedido obtenido correctamente");
    }

    public async Task<ApiResponse<PedidoResponseDto>> CreatePedidoAsync(PedidoRequestDto dto)
    {
        var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == dto.ClienteId);

        if (!clienteExiste)
            throw new BadRequestException($"No existe un cliente con id {dto.ClienteId}.");

        var pedido = new Pedido
        {
            Descripcion = dto.Descripcion,
            ClienteId = dto.ClienteId
        };

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        var pedidoDto = new PedidoResponseDto
        {
            Id = pedido.Id,
            Descripcion = pedido.Descripcion,
            ClienteId = pedido.ClienteId
        };

        _logger.LogInformation("Se creó el pedido con id {Id}.", pedido.Id);

        return ResponseHelper.Success(pedidoDto, "Pedido creado correctamente");
    }

    public async Task<ApiResponseSimple> UpdatePedidoAsync(int id, PedidoRequestDto dto)
    {
        if (id <= 0)
            throw new BadRequestException("El id del pedido debe ser mayor a 0.");

        var pedido = await _context.Pedidos.FindAsync(id);

        if (pedido == null)
            throw new NotFoundException($"No se encontró el pedido con id {id}.");

        var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == dto.ClienteId);

        if (!clienteExiste)
            throw new BadRequestException($"No existe un cliente con id {dto.ClienteId}.");

        pedido.Descripcion = dto.Descripcion;
        pedido.ClienteId = dto.ClienteId;

        _context.Pedidos.Update(pedido);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Se actualizó el pedido con id {Id}.", id);

        return ResponseHelper.Success("Pedido actualizado correctamente");
    }

    public async Task<ApiResponseSimple> DeletePedidoAsync(int id)
    {
        if (id <= 0)
            throw new BadRequestException("El id del pedido debe ser mayor a 0.");

        var pedido = await _context.Pedidos.FindAsync(id);

        if (pedido == null)
            throw new NotFoundException($"No se encontró el pedido con id {id}.");

        _context.Pedidos.Remove(pedido);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Se eliminó el pedido con id {Id}.", id);

        return ResponseHelper.Success("Pedido eliminado correctamente");
    }
}