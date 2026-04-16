using System.ComponentModel.DataAnnotations;

namespace CrudNet10.Dtos;

public class CrearProductoDto
{
    [Required(ErrorMessage = "El nombre del producto es obligatorio"),
        StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres"),
        DataType(DataType.Text)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El precio es obligatorio"),
        Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero"),
        DataType(DataType.Currency), Display(Name = "Precio")]
    public decimal Precio { get; set; }


    [Required(ErrorMessage = "El stock es obligatorio"),
        Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo"), 
        DataType(DataType.Text), Display(Name = "Stock"),
        RegularExpression(@"^\d+$", ErrorMessage = "El stock debe ser un número entero")]
    public int Stock { get; set; }
}
