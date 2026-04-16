using CrudNet10.Data;
using CrudNet10.Dtos;
using CrudNet10.Exceptions;
using CrudNet10.Helpers;
using CrudNet10.Middleware;
using CrudNet10.Models;
using CrudNet10.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrudNet10.Services;

public class ProductoService : IProductoService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductoService> _logger;

    public ProductoService(AppDbContext context, ILogger<ProductoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<ProductoResponseDto>>> GetProductosAsync(PaginationDto paginationDto)
    {
        if (paginationDto.Page <= 0)
            throw new BadRequestException("El número de página debe ser mayor a 0.");

        if (paginationDto.PageSize <= 0)
            throw new BadRequestException("El tamaño de página debe ser mayor a 0.");

        var query = _context.Productos.AsQueryable();

        var totalRecords = await query.CountAsync();

        if (totalRecords == 0)
            throw new NotFoundException("No hay productos registrados.");

        var productos = await query
            .OrderBy(p => p.Id)
            .Skip((paginationDto.Page - 1) * paginationDto.PageSize)
            .Take(paginationDto.PageSize)
            .Select(p => new ProductoResponseDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                Stock = p.Stock
            })
            .ToListAsync();

        var result = new PagedResult<ProductoResponseDto>
        {
            Items = productos,
            Page = paginationDto.Page,
            PageSize = paginationDto.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling((double)totalRecords / paginationDto.PageSize)
        };

        _logger.LogInformation("Se obtuvieron {Cantidad} productos.", productos.Count);

        return ResponseHelper.Success(result, "Productos obtenidos correctamente");
    }

    public async Task<ApiResponse<ProductoResponseDto>> GetProductoByIdAsync(int id)
    {
        if (id <= 0)
            throw new BadRequestException("El id del producto debe ser mayor a 0.");

        var producto = await _context.Productos.FindAsync(id);

        if (producto == null)
            throw new NotFoundException($"No se encontró el producto con id {id}.");

        var productoDto = new ProductoResponseDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Stock = producto.Stock
        };

        return ResponseHelper.Success(productoDto, "Producto obtenido correctamente");
    }

    public async Task<ApiResponse<ProductoResponseDto>> CreateProductoAsync(CrearProductoDto dto)
    {
        var existeNombre = await _context.Productos.AnyAsync(p => p.Nombre == dto.Nombre);

        if (existeNombre)
            throw new BadRequestException("Ya existe un producto con ese nombre.");

        var producto = new Producto
        {
            Nombre = dto.Nombre,
            Precio = dto.Precio,
            Stock = dto.Stock
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        var productoDto = new ProductoResponseDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Stock = producto.Stock
        };

        return ResponseHelper.Success(productoDto, "Producto creado correctamente");
    }

    public async Task<ApiResponseSimple> UpdateProductoAsync(int id, CrearProductoDto dto)
    {
        if (id <= 0)
            throw new BadRequestException("El id del producto debe ser mayor a 0.");

        var producto = await _context.Productos.FindAsync(id);

        if (producto == null)
            throw new NotFoundException($"No se encontró el producto con id {id}.");

        var existeOtroNombre = await _context.Productos.AnyAsync(p => p.Nombre == dto.Nombre && p.Id != id);

        if (existeOtroNombre)
            throw new BadRequestException("Otro producto ya usa ese nombre.");

        producto.Nombre = dto.Nombre;
        producto.Precio = dto.Precio;
        producto.Stock = dto.Stock;

        await _context.SaveChangesAsync();

        return ResponseHelper.Success("Producto actualizado correctamente");
    }

    public async Task<ApiResponseSimple> DeleteProductoAsync(int id)
    {
        if (id <= 0)
            throw new BadRequestException("El id del producto debe ser mayor a 0.");

        var producto = await _context.Productos.FindAsync(id);

        if (producto == null)
            throw new NotFoundException($"No se encontró el producto con id {id}.");

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        return ResponseHelper.Success("Producto eliminado correctamente");
    }
}