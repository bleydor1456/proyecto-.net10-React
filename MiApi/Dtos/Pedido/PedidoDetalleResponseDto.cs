namespace CrudNet10.Dtos;

public class PedidoDetalleResponseDto
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
}