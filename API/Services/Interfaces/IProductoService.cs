using CrudNet10.Dtos;
using CrudNet10.Helpers;

namespace CrudNet10.Services.Interfaces;

public interface IProductoService
{
    Task<ApiResponse<PagedResult<ProductoResponseDto>>> GetProductosAsync(PaginationDto paginationDto);
    Task<ApiResponse<ProductoResponseDto>> GetProductoByIdAsync(int id);
    Task<ApiResponse<ProductoResponseDto>> CreateProductoAsync(CrearProductoDto dto);
    Task<ApiResponseSimple> UpdateProductoAsync(int id, CrearProductoDto dto);
    Task<ApiResponseSimple> DeleteProductoAsync(int id);
}