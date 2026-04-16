using CrudNet10.Dtos;
using CrudNet10.Helpers;
using CrudNet10.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CrudNet10.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UsuarioResponseDto>>> GetMiPerfil()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
            return Unauthorized();

        var response = await _usuarioService.GetByIdAsync(int.Parse(userIdClaim));
        return Ok(response);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UsuarioResponseDto>>>> GetUsuarios()
    {
        var response = await _usuarioService.GetAllAsync();
        return Ok(response);
    }

    [HttpPut("{id}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseSimple>> CambiarRol(int id, CambiarRolDto dto)
    {
        var response = await _usuarioService.CambiarRolAsync(id, dto);
        return Ok(response);
    }

    [HttpPost("crear-admin")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<UsuarioResponseDto>>> CrearAdmin(RegisterDto dto)
    {
        var response = await _usuarioService.CrearAdminAsync(dto);
        return Ok(response);
    }
}