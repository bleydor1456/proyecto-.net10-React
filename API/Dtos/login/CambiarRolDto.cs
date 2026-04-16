using System.ComponentModel.DataAnnotations;

namespace CrudNet10.Dtos;

public class CambiarRolDto
{
    [Required(ErrorMessage = "El rol es obligatorio.")]
    public string Role { get; set; } = string.Empty;
}