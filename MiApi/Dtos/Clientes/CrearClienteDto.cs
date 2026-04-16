using System.ComponentModel.DataAnnotations;

namespace CrudNet10.Dtos;

public class CrearClienteDto
{
    [Required(ErrorMessage = "El nombre del cliente es obligatorio"),
    StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres"),
    DataType(DataType.Text)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email del cliente es obligatorio"),
    EmailAddress(ErrorMessage = "El formato del email no es válido"),
    StringLength(100, ErrorMessage = "El email no puede tener más de 100 caracteres"),
    DataType(DataType.EmailAddress),
    Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono del cliente es obligatorio"),
    RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "El número de teléfono no es válido"),
    StringLength(15, MinimumLength = 7, ErrorMessage = "El número de teléfono debe tener entre 7 y 15 caracteres"),
    DataType(DataType.PhoneNumber),
    Display(Name = "Teléfono")
    ]
    public string Telefono { get; set; } = string.Empty;
}