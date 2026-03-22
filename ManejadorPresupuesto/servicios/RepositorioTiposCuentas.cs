using Dapper;
using ManejadorPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejadorPresupuesto.servicios
{

    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipocuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipocuenta);

        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
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
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO TiposCuentas (Nombre, UsuarioId, Orden)
                                                            VALUES (@Nombre, @UsuarioId, 0);
                                                            SELECT SCOPE_IDENTITY();", tipocuenta);


            tipocuenta.Id = id;
        }




        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                                                                        @"Select 1 from TiposCuentas where Nombre = @Nombre AND UsuarioId = @UsuarioId;", new { nombre, usuarioId });

            return existe == 1;


        }


        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(
                                                                        @"Select Id, Nombre, UsuarioId from TiposCuentas where UsuarioId = @UsuarioId;", new { usuarioId });


        }


        public async Task Actualizar(TipoCuenta tipocuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TiposCuentas SET Nombre = @Nombre WHERE Id = @Id", tipocuenta);
        }

        // permitir tipo de cuenta por ID
        // es para validar que el tipo de cuenta exista y que el usuario tenga permiso para acceder a ese tipo de cuenta
        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(
                                                                        @"Select Id, Nombre, Orden from TiposCuentas where Id = @Id AND UsuarioId = @UsuarioId;", new { id, usuarioId });




        }


        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE FROM TiposCuentas WHERE Id = @Id", new { id });
        }
    }
}
