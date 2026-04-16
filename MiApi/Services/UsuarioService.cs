using CrudNet10.Data;
using CrudNet10.Dtos;
using CrudNet10.Exceptions;
using CrudNet10.Helpers;
using CrudNet10.Middleware;
using CrudNet10.Models;
using CrudNet10.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CrudNet10.Services;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _context;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(AppDbContext context, ILogger<UsuarioService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<UsuarioResponseDto>>> GetAllAsync()
    {
        var usuarios = await _context.Usuarios
            .Select(u => new UsuarioResponseDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();

        if (!usuarios.Any())
            throw new NotFoundException("No hay usuarios registrados.");

        return ResponseHelper.Success<IEnumerable<UsuarioResponseDto>>(usuarios, "Usuarios obtenidos correctamente");
    }

    public async Task<ApiResponse<UsuarioResponseDto>> GetByIdAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);

        if (usuario == null)
            throw new NotFoundException($"No se encontró el usuario con id {id}.");

        var dto = new UsuarioResponseDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Role = usuario.Role
        };

        return ResponseHelper.Success(dto, "Usuario obtenido correctamente");
    }

    public async Task<ApiResponseSimple> CambiarRolAsync(int id, CambiarRolDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(id);

        if (usuario == null)
            throw new NotFoundException($"No se encontró el usuario con id {id}.");

        var rol = dto.Role.Trim();

        if (rol != "Admin" && rol != "Usuario")
            throw new BadRequestException("El rol solo puede ser 'Admin' o 'Usuario'.");

        usuario.Role = rol;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Se cambió el rol del usuario {Id} a {Rol}.", id, rol);

        return ResponseHelper.Success("Rol actualizado correctamente");
    }

    public async Task<ApiResponse<UsuarioResponseDto>> CrearAdminAsync(RegisterDto dto)
    {
        var existeUsuario = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);

        if (existeUsuario)
            throw new BadRequestException("Ya existe un usuario con ese email.");

        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            Role = "Admin"
        };

        var passwordHasher = new PasswordHasher<Usuario>();
        usuario.PasswordHash = passwordHasher.HashPassword(usuario, dto.Password);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var usuarioDto = new UsuarioResponseDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Role = usuario.Role
        };

        _logger.LogInformation("Se creó un nuevo admin con id {Id}.", usuario.Id);

        return ResponseHelper.Success(usuarioDto, "Administrador creado correctamente");
    }
}