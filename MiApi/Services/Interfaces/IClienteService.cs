using CrudNet10.Dtos;
using CrudNet10.Helpers;

namespace CrudNet10.Services.Interfaces;

public interface IClienteService
{
    Task<ApiResponse<PagedResult<ClienteResponseDto>>> GetClientesAsync(PaginationDto paginationDto);
    Task<ApiResponse<ClienteDetalleResponseDto>> GetClienteByIdAsync(int id);
    Task<ApiResponse<ClienteResponseDto>> CreateClienteAsync(CrearClienteDto dto, int userId);
    Task<ApiResponseSimple> UpdateClienteAsync(int id, CrearClienteDto dto, int userId, bool esAdmin);
    Task<ApiResponseSimple> DeleteClienteAsync(int id, int userId, bool esAdmin);
}