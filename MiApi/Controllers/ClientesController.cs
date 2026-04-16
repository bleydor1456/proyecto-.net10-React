using CrudNet10.Dtos;
using CrudNet10.Helpers;
using CrudNet10.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrudNet10.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PagedResult<ClienteResponseDto>>>> GetClientes([FromQuery] PaginationDto paginationDto)
    {
        var response = await _clienteService.GetClientesAsync(paginationDto);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<ApiResponse<ClienteResponseDto>>> GetCliente(int id)
    {
        var response = await _clienteService.GetClienteByIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ClienteResponseDto>>> PostCliente(CrearClienteDto dto)
    {
        var userId = GetUserId();

        var response = await _clienteService.CreateClienteAsync(dto, userId);
        return Ok(response);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponseSimple>> PutCliente(int id, CrearClienteDto dto)
    {

        var userId = GetUserId();
        var esAdmin = EsAdmin();

        var response = await _clienteService.UpdateClienteAsync(id, dto, userId, esAdmin);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponseSimple>> DeleteCliente(int id)
    {

        var userId = GetUserId();
        var esAdmin = EsAdmin();

        var response = await _clienteService.DeleteClienteAsync(id, userId, esAdmin);
        return Ok(response);
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
            throw new UnauthorizedAccessException("Usuario no autenticado.");

        return int.Parse(userIdClaim);
    }

    private bool EsAdmin()
    {
        return User.IsInRole("Admin");
    }
}