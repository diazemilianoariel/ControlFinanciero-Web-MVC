using Dapper;
using ManejadorPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejadorPresupuesto.servicios
{

    public interface IRepositorioTiposCuentas
    {


        void Crear(TipoCuenta tipocuenta);
    }



    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {

        private readonly string connectionString;


     
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public void Crear(TipoCuenta tipocuenta)
        {

            using var connection = new SqlConnection(connectionString);
            var id = connection.QuerySingle<int>(@"INSERT INTO TiposCuentas (Nombre, UsuarioId, Orden)
                                                            VALUES (@Nombre, @UsuarioId, 0);
                                                            SELECT SCOPE_IDENTITY();", tipocuenta);


            tipocuenta.Id = id;
        }


















    }
}
