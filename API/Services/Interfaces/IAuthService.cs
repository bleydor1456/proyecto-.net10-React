using CrudNet10.Dtos;
using CrudNet10.Helpers;

namespace CrudNet10.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<UsuarioResponseDto>> RegisterAsync(RegisterDto dto);
    Task<ApiResponse<string>> LoginAsync(LoginDto dto);
}