using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using CampusLove.Infrastructure.Configuration;
using CampusLove2.Domain.Entities;

namespace CampusLove.Infrastructure.Repositories
{
    public class ReaccionesRepository
    {
        private readonly MySqlConnection _connection;

        public ReaccionesRepository()
        {
            _connection = DatabaseConfig.GetConnection();
        }

        public async Task<IEnumerable<Reacciones>> GetAllAsync()
        {
            var reacciones = new List<Reacciones>();
            const string query = @"
                SELECT r.*, u.username as usuario_nombre, p.nombre as perfil_nombre 
                FROM reacciones r 
                INNER JOIN usuarios u ON r.id_usuarios = u.id_usuarios 
                INNER JOIN perfil p ON r.id_perfil = p.id_perfil";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                reacciones.Add(new Reacciones
                {
                    IdReacciones = Convert.ToInt32(reader["id_reacciones"]),
                    IdUsuarios = Convert.ToInt32(reader["id_usuarios"]),
                    IdPerfil = Convert.ToInt32(reader["id_perfil"]),
                    Tipo = reader["tipo"].ToString(),
                    FechaReaccion = Convert.ToDateTime(reader["fecha_reaccion"])
                });
            }

            return reacciones;
        }

        public async Task<Reacciones?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT r.*, u.username as usuario_nombre, p.nombre as perfil_nombre 
                FROM reacciones r 
                INNER JOIN usuarios u ON r.id_usuarios = u.id_usuarios 
                INNER JOIN perfil p ON r.id_perfil = p.id_perfil 
                WHERE r.id_reacciones = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Reacciones
                {
                    IdReacciones = Convert.ToInt32(reader["id_reacciones"]),
                    IdUsuarios = Convert.ToInt32(reader["id_usuarios"]),
                    IdPerfil = Convert.ToInt32(reader["id_perfil"]),
                    Tipo = reader["tipo"].ToString(),
                    FechaReaccion = Convert.ToDateTime(reader["fecha_reaccion"])
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Reacciones reaccion)
        {
            if (reaccion == null)
                throw new ArgumentNullException(nameof(reaccion));

            const string query = @"
                INSERT INTO reacciones (id_usuarios, id_perfil, tipo, fecha_reaccion) 
                VALUES (@IdUsuario, @IdPerfil, @Tipo, @FechaReaccion)";
            
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@IdUsuario", reaccion.IdUsuarios);
                command.Parameters.AddWithValue("@IdPerfil", reaccion.IdPerfil);
                command.Parameters.AddWithValue("@Tipo", reaccion.Tipo);
                command.Parameters.AddWithValue("@FechaReaccion", reaccion.FechaReaccion);

                var result = await command.ExecuteNonQueryAsync() > 0;

                // Actualizar total_likes en perfil si es un like
                if (reaccion.Tipo == "Like")
                {
                    // Primero verificamos el valor actual de total_likes para evitar problemas con NULL
                    const string checkLikesQuery = @"
                        SELECT COALESCE(total_likes, 0) FROM perfil 
                        WHERE id_perfil = @IdPerfil";
                    
                    using var checkCommand = new MySqlCommand(checkLikesQuery, _connection, transaction);
                    checkCommand.Parameters.AddWithValue("@IdPerfil", reaccion.IdPerfil);
                    var currentLikes = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());
                    
                    // Ahora actualizamos con el valor correcto
                    const string updateLikesQuery = @"
                        UPDATE perfil 
                        SET total_likes = @NewTotalLikes 
                        WHERE id_perfil = @IdPerfil";

                    using var updateCommand = new MySqlCommand(updateLikesQuery, _connection, transaction);
                    updateCommand.Parameters.AddWithValue("@NewTotalLikes", currentLikes + 1);
                    updateCommand.Parameters.AddWithValue("@IdPerfil", reaccion.IdPerfil);
                    await updateCommand.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Reacciones reaccion)
        {
            if (reaccion == null)
                throw new ArgumentNullException(nameof(reaccion));

            const string query = @"
                UPDATE reacciones 
                SET id_usuarios = @IdUsuario, 
                    id_perfil = @IdPerfil, 
                    tipo = @Tipo, 
                    fecha_reaccion = @FechaReaccion 
                WHERE id_reacciones = @Id";

            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                // Primero obtener el tipo anterior de la reacción
                const string getOldTypeQuery = "SELECT tipo FROM reacciones WHERE id_reacciones = @Id";
                using var getOldTypeCommand = new MySqlCommand(getOldTypeQuery, _connection, transaction);
                getOldTypeCommand.Parameters.AddWithValue("@Id", reaccion.IdReacciones);
                var oldType = (string)await getOldTypeCommand.ExecuteScalarAsync();

                // Actualizar la reacción
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@IdUsuario", reaccion.IdUsuarios);
                command.Parameters.AddWithValue("@IdPerfil", reaccion.IdPerfil);
                command.Parameters.AddWithValue("@Tipo", reaccion.Tipo);
                command.Parameters.AddWithValue("@FechaReaccion", reaccion.FechaReaccion);
                command.Parameters.AddWithValue("@Id", reaccion.IdReacciones);

                var result = await command.ExecuteNonQueryAsync() > 0;

                // Actualizar total_likes en perfil si cambió el tipo de reacción
                if (oldType != reaccion.Tipo)
                {
                    const string updateLikesQuery = @"
                        UPDATE perfil 
                        SET total_likes = total_likes + CASE 
                            WHEN @NewType = 'Like' THEN 1 
                            WHEN @OldType = 'Like' THEN -1 
                            ELSE 0 
                        END 
                        WHERE id_perfil = @IdPerfil";

                    using var updateCommand = new MySqlCommand(updateLikesQuery, _connection, transaction);
                    updateCommand.Parameters.AddWithValue("@NewType", reaccion.Tipo);
                    updateCommand.Parameters.AddWithValue("@OldType", oldType);
                    updateCommand.Parameters.AddWithValue("@IdPerfil", reaccion.IdPerfil);
                    await updateCommand.ExecuteNonQueryAsync();
                }

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
            const string query = "DELETE FROM reacciones WHERE id_reacciones = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                // Primero obtener la información de la reacción
                const string getReaccionQuery = "SELECT tipo, id_perfil FROM reacciones WHERE id_reacciones = @Id";
                using var getReaccionCommand = new MySqlCommand(getReaccionQuery, _connection, transaction);
                getReaccionCommand.Parameters.AddWithValue("@Id", id);
                
                using var reader = await getReaccionCommand.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var tipo = reader["tipo"].ToString();
                    var idPerfil = Convert.ToInt32(reader["id_perfil"]);

                    // Eliminar la reacción
                    using var deleteCommand = new MySqlCommand(query, _connection, transaction);
                    deleteCommand.Parameters.AddWithValue("@Id", id);
                    var result = await deleteCommand.ExecuteNonQueryAsync() > 0;

                    // Actualizar total_likes en perfil si era un like
                    if (tipo == "Like")
                    {
                        const string updateLikesQuery = @"
                            UPDATE perfil 
                            SET total_likes = total_likes - 1 
                            WHERE id_perfil = @IdPerfil";

                        using var updateCommand = new MySqlCommand(updateLikesQuery, _connection, transaction);
                        updateCommand.Parameters.AddWithValue("@IdPerfil", idPerfil);
                        await updateCommand.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();
                    return result;
                }

                await transaction.RollbackAsync();
                return false;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Reacciones>> GetReaccionesByUsuarioAsync(int idUsuario)
        {
            var reacciones = new List<Reacciones>();
            const string query = @"
                SELECT r.*, u.username as usuario_nombre, p.nombre as perfil_nombre 
                FROM reacciones r 
                INNER JOIN usuarios u ON r.id_usuarios = u.id_usuarios 
                INNER JOIN perfil p ON r.id_perfil = p.id_perfil 
                WHERE r.id_usuarios = @IdUsuario";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@IdUsuario", idUsuario);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                reacciones.Add(new Reacciones
                {
                    IdReacciones = Convert.ToInt32(reader["id_reacciones"]),
                    IdUsuarios = Convert.ToInt32(reader["id_usuarios"]),
                    IdPerfil = Convert.ToInt32(reader["id_perfil"]),
                    Tipo = reader["tipo"].ToString(),
                    FechaReaccion = Convert.ToDateTime(reader["fecha_reaccion"])
                });
            }

            return reacciones;
        }

        // Verifica si existe un match (likes mutuos) entre dos usuarios
        public async Task<bool> ExisteMatchAsync(int idUsuario1, int idUsuario2)
        {
            // Verificar si el usuario1 le dio like al usuario2
            const string query1 = @"
                SELECT COUNT(*) FROM reacciones r
                INNER JOIN usuarios u ON r.id_usuarios = u.id_usuarios
                INNER JOIN perfil p ON r.id_perfil = p.id_perfil
                WHERE r.id_usuarios = @IdUsuario1 
                AND p.id_perfil = @IdUsuario2
                AND r.tipo = 'Like'";

            using var command1 = new MySqlCommand(query1, _connection);
            command1.Parameters.AddWithValue("@IdUsuario1", idUsuario1);
            command1.Parameters.AddWithValue("@IdUsuario2", idUsuario2);
            var count1 = Convert.ToInt32(await command1.ExecuteScalarAsync());

            if (count1 == 0) return false;

            // Verificar si el usuario2 le dio like al usuario1
            const string query2 = @"
                SELECT COUNT(*) FROM reacciones r
                INNER JOIN usuarios u ON r.id_usuarios = u.id_usuarios
                INNER JOIN perfil p ON r.id_perfil = p.id_perfil
                WHERE r.id_usuarios = @IdUsuario2 
                AND p.id_perfil = @IdUsuario1
                AND r.tipo = 'Like'";

            using var command2 = new MySqlCommand(query2, _connection);
            command2.Parameters.AddWithValue("@IdUsuario1", idUsuario1);
            command2.Parameters.AddWithValue("@IdUsuario2", idUsuario2);
            var count2 = Convert.ToInt32(await command2.ExecuteScalarAsync());

            return count2 > 0;
        }

        // Obtiene los perfiles que han dado like al usuario y a los que el usuario ha dado like (matches potenciales)
        public async Task<IEnumerable<int>> GetPotentialMatchesAsync(int idUsuario)
        {
            var potentialMatches = new List<int>();
            
            // Obtener los perfiles a los que el usuario ha dado like
            const string query = @"
                SELECT p.id_perfil 
                FROM reacciones r 
                INNER JOIN perfil p ON r.id_perfil = p.id_perfil 
                WHERE r.id_usuarios = @IdUsuario AND r.tipo = 'Like'";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@IdUsuario", idUsuario);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                potentialMatches.Add(Convert.ToInt32(reader["id_perfil"]));
            }

            return potentialMatches;
        }
    }
}