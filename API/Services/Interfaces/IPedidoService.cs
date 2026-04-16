using CrudNet10.Dtos;
using CrudNet10.Helpers;

namespace CrudNet10.Services.Interfaces;

public interface IPedidoService
{
    Task<ApiResponse<PagedResult<PedidoResponseDto>>> GetPedidosAsync(PaginationDto paginationDto);
    Task<ApiResponse<PedidoResponseDto>> GetPedidoByIdAsync(int id);
    Task<ApiResponse<PedidoResponseDto>> CreatePedidoAsync(PedidoRequestDto dto);
    Task<ApiResponseSimple> UpdatePedidoAsync(int id, PedidoRequestDto dto);
    Task<ApiResponseSimple> DeletePedidoAsync(int id);
}