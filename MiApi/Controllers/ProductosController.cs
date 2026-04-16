using CrudNet10.Dtos;
using CrudNet10.Helpers;
using CrudNet10.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrudNet10.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductoResponseDto>>>> GetProductos([FromQuery] PaginationDto paginationDto)
    {
        var response = await _productoService.GetProductosAsync(paginationDto);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductoResponseDto>>> GetProducto(int id)
    {
        var response = await _productoService.GetProductoByIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductoResponseDto>>> PostProducto(CrearProductoDto dto)
    {
        var response = await _productoService.CreateProductoAsync(dto);
        return Ok(response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseSimple>> PutProducto(int id, CrearProductoDto dto)
    {
        var response = await _productoService.UpdateProductoAsync(id, dto);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseSimple>> DeleteProducto(int id)
    {
        var response = await _productoService.DeleteProductoAsync(id);
        return Ok(response);
    }
}