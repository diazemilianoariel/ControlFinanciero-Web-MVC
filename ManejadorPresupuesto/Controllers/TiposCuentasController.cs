using Dapper;
using ManejadorPresupuesto.Models;
using ManejadorPresupuesto.servicios;
using Microsoft.AspNetCore.Mvc;


namespace ManejadorPresupuesto.Controllers
{


    public class TiposCuentasController : Controller // Controlador para manejar las operaciones relacionadas con los tipos de cuentas en la aplicación de presupuesto.
    {

        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;

        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }


        // Acción para mostrar la lista de tipos de cuentas.
        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            return View(tiposCuentas);
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



            tipocuenta.UsuarioId = servicioUsuarios.ObtenerUsuarioId();



            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(tipocuenta.Nombre, tipocuenta.UsuarioId);


            if (yaExisteTipoCuenta)
            {
                ModelState.AddModelError(nameof(tipocuenta.Nombre), $"El nombre {tipocuenta.Nombre} ya existe");
                return View(tipocuenta);
            }


            await repositorioTiposCuentas.Crear(tipocuenta);
            return RedirectToAction("Index");
        }


        // me va apermitir editar un tipo de cuenta, pero primero tengo que validar que el tipo de cuenta exista y que el usuario tenga permiso para acceder a ese tipo de cuenta, por eso necesito el ID del tipo de cuenta y el ID del usuario, para validar que el tipo de cuenta exista y que el usuario tenga permiso para acceder a ese tipo de cuenta, si no existe o no tiene permiso, redirijo a una página de error, si existe y tiene permiso, muestro la vista para editar el tipo de cuenta.
        [HttpGet]
        public async Task<ActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }



        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipocuenta)
        {

            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tipocuenta.Id, usuarioId);

            if (tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }


            await repositorioTiposCuentas.Actualizar(tipocuenta);
            return RedirectToAction("Index");
        }





        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);

        }

        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTiposCuentas.Borrar(id);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(nombre, usuarioId);
            if (yaExisteTipoCuenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);

        }



        // no puedo confiar en la data que viene del  usario es por eso que debo validar que los IDs que vienen del usuario pertenecen al usuario, para eso necesito obtener los tipos de cuentas del usuario y comparar los IDs que vienen del usuario con los IDs de los tipos de cuentas del usuario, si hay algún ID que no pertenece al usuario, entonces no puedo confiar en la data y debo retornar un error, si todos los IDs pertenecen al usuario, entonces puedo confiar en la data y puedo actualizar el orden de los tipos de cuentas.
        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {

            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);

            // aca lo que hago es obtener los IDs de los tipos de cuentas del usuario y compararlos con los IDs que vienen del usuario, si hay algún ID que no pertenece al usuario, entonces no puedo confiar en la data y debo retornar un error, si todos los IDs pertenecen al usuario, entonces puedo confiar en la data y puedo actualizar el orden de los tipos de cuentas.
            var idsTipoCuentas = tiposCuentas.Select(x => x.Id);
            var idsTipoCuentasNoPertenecenAlUsuario = ids.Except(idsTipoCuentas).ToList();

            if(idsTipoCuentasNoPertenecenAlUsuario.Count() > 0)
            {
                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select((valor, indice) => new TipoCuenta() { Id = valor, Orden = indice + 1 }).AsEnumerable();

            await repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);

            return Ok();
        }
    }
}
