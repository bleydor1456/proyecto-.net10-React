using CrudNet10.Dtos;
using CrudNet10.Helpers;
using CrudNet10.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrudNet10.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PedidoResponseDto>>>> GetPedidos([FromQuery] PaginationDto paginationDto)
    {
        var response = await _pedidoService.GetPedidosAsync(paginationDto);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<PedidoResponseDto>>> GetPedido(int id)
    {
        var response = await _pedidoService.GetPedidoByIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PedidoResponseDto>>> PostPedido(PedidoRequestDto dto)
    {
        var response = await _pedidoService.CreatePedidoAsync(dto);
        return Ok(response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseSimple>> PutPedido(int id, PedidoRequestDto dto)
    {
        var response = await _pedidoService.UpdatePedidoAsync(id, dto);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
   public async Task<ActionResult<ApiResponseSimple>> DeletePedido(int id)
    {
        var response = await _pedidoService.DeletePedidoAsync(id);
        return Ok(response);
    }
}