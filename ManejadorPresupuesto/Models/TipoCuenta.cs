
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejadorPresupuesto.Models
{

    public class TipoCuenta : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
        [Display(Name = "Nombre de tipo Cuenta")]

        //[PrimeraLetraMayuscula]

        [Remote(action: "VerificarExisteTipoCuenta", controller: "TiposCuentas")]
        public string Nombre { get; set; }

        public int UsuarioId { get; set; }  

        public int Orden { get; set; }




        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre) && char.IsLower(Nombre[0]))
            {
                yield return new ValidationResult("La primera letra debe ser mayúscula", new[] { nameof(Nombre) });
            }
        }

    }
}
