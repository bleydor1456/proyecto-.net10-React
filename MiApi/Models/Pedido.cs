namespace CrudNet10.Models;

public class Pedido
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
}