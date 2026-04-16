namespace CrudNet10.Dtos;

public class ClienteDetalleResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public List<PedidoResponseDto> Pedidos { get; set; } = new();
}