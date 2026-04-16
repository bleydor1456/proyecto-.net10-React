using System.ComponentModel.DataAnnotations;

namespace CrudNet10.Dtos;

public class PedidoRequestDto
{
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(300, ErrorMessage = "La descripción no puede tener más de 300 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El id del cliente es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El id del cliente debe ser mayor a 0.")]
    public int ClienteId { get; set; }
}