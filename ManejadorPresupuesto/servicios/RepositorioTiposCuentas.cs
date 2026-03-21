using Dapper;
using ManejadorPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejadorPresupuesto.servicios
{

    public interface IRepositorioTiposCuentas
    {


        Task Crear(TipoCuenta tipocuenta);

        Task<bool> Existe(string nombre, int usuarioId);
    }



    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {

        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task Crear(TipoCuenta tipocuenta)
        {

            using var connection = new SqlConnection(connectionString);
            var id  = await connection.QuerySingleAsync<int>(@"INSERT INTO TiposCuentas (Nombre, UsuarioId, Orden)
                                                            VALUES (@Nombre, @UsuarioId, 0);
                                                            SELECT SCOPE_IDENTITY();", tipocuenta);


            tipocuenta.Id = id;
        }




        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                                                                        @"Select 1 from TiposCuentas where Nombre = @Nombre AND UsuarioId = @UsuarioId;", new {nombre,usuarioId});

            return existe == 1;


        }

    }




}
