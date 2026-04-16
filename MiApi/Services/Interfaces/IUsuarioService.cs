using CrudNet10.Dtos;
using CrudNet10.Helpers;

namespace CrudNet10.Services.Interfaces;

public interface IUsuarioService
{
    Task<ApiResponse<IEnumerable<UsuarioResponseDto>>> GetAllAsync();
    Task<ApiResponse<UsuarioResponseDto>> GetByIdAsync(int id);
    Task<ApiResponseSimple> CambiarRolAsync(int id, CambiarRolDto dto);
    Task<ApiResponse<UsuarioResponseDto>> CrearAdminAsync(RegisterDto dto);
}