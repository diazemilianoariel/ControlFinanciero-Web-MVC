using Dapper;
using ManejadorPresupuesto.Models;
using ManejadorPresupuesto.servicios;
using Microsoft.AspNetCore.Mvc;


namespace ManejadorPresupuesto.Controllers
{


    public class TiposCuentasController : Controller // Controlador para manejar las operaciones relacionadas con los tipos de cuentas en la aplicación de presupuesto.
    {

        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;


        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
        }

        // Acción para mostrar la vista de creación de un nuevo tipo de cuenta.
        // si el de abajo es un HttpPost, este es un HttpGet, que se encarga de mostrar la vista para crear un nuevo tipo de cuenta.
        public IActionResult Crear()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipocuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipocuenta);
            }



            tipocuenta.UsuarioId = 1;



            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(tipocuenta.Nombre, tipocuenta.UsuarioId);


            if (yaExisteTipoCuenta)
            {
                ModelState.AddModelError(nameof(tipocuenta.Nombre), $"El nombre {tipocuenta.Nombre} ya existe");
                return View(tipocuenta);
            }


            await repositorioTiposCuentas.Crear(tipocuenta);
            return View();
        }



        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var usuarioId = 1;
            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(nombre, usuarioId);
            if (yaExisteTipoCuenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);

        }
    }
}
