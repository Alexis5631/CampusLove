using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using CampusLove.Infrastructure.Configuration;
using CampusLove2.Domain.Entities;

namespace CampusLove.Infrastructure.Repositories
{
    public class PerfilRepository
    {
        private readonly MySqlConnection _connection;

        public PerfilRepository()
        {
            _connection = DatabaseConfig.GetConnection();
        }

        public async Task<IEnumerable<Perfil>> GetAllAsync()
        {
            var perfilList = new List<Perfil>();
            const string query = @"
                SELECT p.*, g.descripcion as genero_descripcion, 
                       e.descripcion as estado_descripcion, 
                       pr.descripcion as profesion_descripcion, 
                       c.Nombre as ciudad_nombre, 
                       r.Nombre as region_nombre, 
                       pa.nombre as pais_nombre,
                       COALESCE((SELECT COUNT(*) FROM reacciones r WHERE r.id_perfil = p.id_perfil AND r.tipo = 'like'), 0) as total_likes
                FROM perfil p 
                LEFT JOIN genero g ON p.id_genero = g.id_genero 
                LEFT JOIN estado e ON p.id_estado = e.id_estado 
                LEFT JOIN profesion pr ON p.id_profesion = pr.id_profesion 
                LEFT JOIN ciudad c ON p.id_ciudad = c.id_ciudad 
                LEFT JOIN region r ON c.id_region = r.id_region 
                LEFT JOIN pais pa ON r.id_pais = pa.id_pais";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                perfilList.Add(new Perfil
                {
                    IdPerfil = Convert.ToInt32(reader["id_perfil"]),
                    Nombre = reader["nombre"].ToString() ?? string.Empty,
                    Apellido = reader["apellido"].ToString() ?? string.Empty,
                    Identificacion = reader["identificacion"].ToString() ?? string.Empty,
                    Biografia = reader["biografia"].ToString() ?? string.Empty,
                    TotalLikes = reader["total_likes"] == DBNull.Value ? 0 : Convert.ToInt32(reader["total_likes"]),
                    Edad = reader["edad"] == DBNull.Value ? 0 : Convert.ToInt32(reader["edad"]),
                    IdGenero = reader["id_genero"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_genero"]),
                    IdEstado = reader["id_estado"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_estado"]),
                    IdProfesion = reader["id_profesion"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_profesion"]),
                    IdCiudad = reader["id_ciudad"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_ciudad"])
                });
            }

            return perfilList;
        }

        public async Task<Perfil?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT p.*, g.descripcion as genero_descripcion, 
                       e.descripcion as estado_descripcion, 
                       pr.descripcion as profesion_descripcion, 
                       c.Nombre as ciudad_nombre, 
                       r.Nombre as region_nombre, 
                       pa.nombre as pais_nombre,
                       COALESCE((SELECT COUNT(*) FROM reacciones r WHERE r.id_perfil = p.id_perfil AND r.tipo = 'like'), 0) as total_likes
                FROM perfil p 
                LEFT JOIN genero g ON p.id_genero = g.id_genero 
                LEFT JOIN estado e ON p.id_estado = e.id_estado 
                LEFT JOIN profesion pr ON p.id_profesion = pr.id_profesion 
                LEFT JOIN ciudad c ON p.id_ciudad = c.id_ciudad 
                LEFT JOIN region r ON c.id_region = r.id_region 
                LEFT JOIN pais pa ON r.id_pais = pa.id_pais 
                WHERE p.id_perfil = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Perfil
                {
                    IdPerfil = reader["id_perfil"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_perfil"]),
                    Nombre = reader["nombre"].ToString() ?? string.Empty,
                    Apellido = reader["apellido"].ToString() ?? string.Empty,
                    Identificacion = reader["identificacion"].ToString() ?? string.Empty,
                    Biografia = reader["biografia"].ToString() ?? string.Empty,
                    TotalLikes = reader["total_likes"] == DBNull.Value ? 0 : Convert.ToInt32(reader["total_likes"]),
                    Edad = reader["edad"] == DBNull.Value ? 0 : Convert.ToInt32(reader["edad"]),
                    IdGenero = reader["id_genero"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_genero"]),
                    IdEstado = reader["id_estado"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_estado"]),
                    IdProfesion = reader["id_profesion"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_profesion"]),
                    IdCiudad = reader["id_ciudad"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_ciudad"])
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Perfil perfil)
        {
            if (perfil == null)
                throw new ArgumentNullException(nameof(perfil));

            const string query = @"
                INSERT INTO perfil (nombre, apellido, identificacion, biografia, total_likes, edad,
                                  id_genero, id_estado, id_profesion, id_ciudad) 
                VALUES (@Nombre, @Apellido, @Identificacion, @Biografia, @TotalLikes, @Edad,
                        @IdGenero, @IdEstado, @IdProfesion, @IdCiudad)";
            
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Nombre", perfil.Nombre);
                command.Parameters.AddWithValue("@Apellido", perfil.Apellido);
                command.Parameters.AddWithValue("@Identificacion", perfil.Identificacion);
                command.Parameters.AddWithValue("@Biografia", perfil.Biografia);
                command.Parameters.AddWithValue("@TotalLikes", perfil.TotalLikes);
                command.Parameters.AddWithValue("@Edad", perfil.Edad);
                command.Parameters.AddWithValue("@IdGenero", perfil.IdGenero);
                command.Parameters.AddWithValue("@IdEstado", perfil.IdEstado);
                command.Parameters.AddWithValue("@IdProfesion", perfil.IdProfesion);
                command.Parameters.AddWithValue("@IdCiudad", perfil.IdCiudad);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Perfil perfil)
        {
            if (perfil == null)
                throw new ArgumentNullException(nameof(perfil));

            const string query = @"
                UPDATE perfil 
                SET nombre = @Nombre, 
                    apellido = @Apellido, 
                    identificacion = @Identificacion, 
                    biografia = @Biografia, 
                    total_likes = @TotalLikes, 
                    edad = @Edad,
                    id_genero = @IdGenero, 
                    id_estado = @IdEstado, 
                    id_profesion = @IdProfesion, 
                    id_ciudad = @IdCiudad 
                WHERE id_perfil = @Id";

            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Nombre", perfil.Nombre);
                command.Parameters.AddWithValue("@Apellido", perfil.Apellido);
                command.Parameters.AddWithValue("@Identificacion", perfil.Identificacion);
                command.Parameters.AddWithValue("@Biografia", perfil.Biografia);
                command.Parameters.AddWithValue("@TotalLikes", perfil.TotalLikes);
                command.Parameters.AddWithValue("@Edad", perfil.Edad);
                command.Parameters.AddWithValue("@IdGenero", perfil.IdGenero);
                command.Parameters.AddWithValue("@IdEstado", perfil.IdEstado);
                command.Parameters.AddWithValue("@IdProfesion", perfil.IdProfesion);
                command.Parameters.AddWithValue("@IdCiudad", perfil.IdCiudad);
                command.Parameters.AddWithValue("@Id", perfil.IdPerfil);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM perfil WHERE id_perfil = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Id", id);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> IncrementarLikesAsync(int idPerfil)
        {
            const string query = "UPDATE perfil SET total_likes = total_likes + 1 WHERE id_perfil = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Id", idPerfil);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DecrementarLikesAsync(int idPerfil)
        {
            const string query = "UPDATE perfil SET total_likes = total_likes - 1 WHERE id_perfil = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Id", idPerfil);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> ActualizarEdadAsync(int idPerfil, int edad)
        {
            const string query = "UPDATE perfil SET edad = @Edad WHERE id_perfil = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Edad", edad);
                command.Parameters.AddWithValue("@Id", idPerfil);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
