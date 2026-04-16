namespace CrudNet10.Dtos;

public class PedidoResponseDto
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int ClienteId { get; set; }
}