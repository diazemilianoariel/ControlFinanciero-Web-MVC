using Dapper;
using ManejadorPresupuesto.Models;
using ManejadorPresupuesto.servicios;
using Microsoft.AspNetCore.Mvc;


namespace ManejadorPresupuesto.Controllers
{

 
    public class TiposCuentasController : Controller
    {

        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;

       
        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas) 
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            

        }
       
        public IActionResult Crear()
        {
                return View();
        }



        [HttpPost]
        public IActionResult Crear(TipoCuenta tipocuenta)
        {

            if (!ModelState.IsValid)
            {


                return View(tipocuenta);


            }


            tipocuenta.UsuarioId = 1;

            repositorioTiposCuentas.Crear(tipocuenta);
            return View();




            
        }

    }
}
