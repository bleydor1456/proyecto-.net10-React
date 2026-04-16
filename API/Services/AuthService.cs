using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CrudNet10.Data;
using CrudNet10.Dtos;
using CrudNet10.Exceptions;
using CrudNet10.Helpers;
using CrudNet10.Middleware;
using CrudNet10.Models;
using CrudNet10.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CrudNet10.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ApiResponse<UsuarioResponseDto>> RegisterAsync(RegisterDto dto)
    {
        var existeUsuario = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);

        if (existeUsuario)
            throw new BadRequestException("Ya existe un usuario con ese email.");

        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            Role = "Usuario"
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

        _logger.LogInformation("Se registró el usuario con id {Id}.", usuario.Id);

        return ResponseHelper.Success(usuarioDto, "Usuario registrado correctamente");
    }

    public async Task<ApiResponse<string>> LoginAsync(LoginDto dto)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (usuario == null)
            throw new BadRequestException("Credenciales inválidas.");

        var passwordHasher = new PasswordHasher<Usuario>();
        var resultado = passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, dto.Password);

        if (resultado == PasswordVerificationResult.Failed)
            throw new BadRequestException("Credenciales inválidas.");

        var jwt = _configuration.GetSection("Jwt");
        var keyString = jwt["Key"];

        if (string.IsNullOrWhiteSpace(keyString))
            throw new Exception("La clave JWT no está configurada.");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        _logger.LogInformation("Login exitoso para el usuario {Email}.", usuario.Email);

        return ResponseHelper.Success(tokenString, "Login exitoso");
    }
}