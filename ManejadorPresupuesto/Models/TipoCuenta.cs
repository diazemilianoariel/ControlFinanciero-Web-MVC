using ManejadorPresupuesto.validaviones;
using System.ComponentModel.DataAnnotations;

namespace ManejadorPresupuesto.Models
{

    public class TipoCuenta 
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "el campo {0} es requerido")]
        public string Nombre { get; set; }
  
        public int UsuarioId { get; set; }
       
        public int Orden { get; set; }
    }
}
