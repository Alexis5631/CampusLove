using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using CampusLove.Infrastructure.Configuration;
using CampusLove2.Domain.Entities;

namespace CampusLove.Infrastructure.Repositories
{
    public class LikesDiariosRepository
    {
        private readonly MySqlConnection _connection;

        public LikesDiariosRepository()
        {
            _connection = DatabaseConfig.GetConnection();
        }

        public async Task<IEnumerable<LikesDiarios>> GetAllAsync()
        {
            var likesDiarios = new List<LikesDiarios>();
            const string query = @"
                SELECT ld.*, p.nombre as perfil_nombre 
                FROM likes_diarios ld 
                INNER JOIN perfil p ON ld.id_perfil = p.id_perfil";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                likesDiarios.Add(new LikesDiarios
                {
                    IdLikesDiarios = Convert.ToInt32(reader["id_likes_diarios"]),
                    Fecha = Convert.ToDateTime(reader["fecha"]),
                    IdPerfil = Convert.ToInt32(reader["id_perfil"]),
                    NumeroLikes = Convert.ToInt32(reader["numero_likes"])
                });
            }

            return likesDiarios;
        }

        public async Task<LikesDiarios?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT ld.*, p.nombre as perfil_nombre 
                FROM likes_diarios ld 
                INNER JOIN perfil p ON ld.id_perfil = p.id_perfil 
                WHERE ld.id_likes_diarios = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new LikesDiarios
                {
                    IdLikesDiarios = Convert.ToInt32(reader["id_likes_diarios"]),
                    Fecha = Convert.ToDateTime(reader["fecha"]),
                    IdPerfil = Convert.ToInt32(reader["id_perfil"]),
                    NumeroLikes = Convert.ToInt32(reader["numero_likes"])
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(LikesDiarios likesDiarios)
        {
            if (likesDiarios == null)
                throw new ArgumentNullException(nameof(likesDiarios));

            const string query = @"
                INSERT INTO likes_diarios (fecha, id_perfil, numero_likes) 
                VALUES (@Fecha, @IdPerfil, @NumeroLikes)";
            
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Fecha", likesDiarios.Fecha);
                command.Parameters.AddWithValue("@IdPerfil", likesDiarios.IdPerfil);
                command.Parameters.AddWithValue("@NumeroLikes", likesDiarios.NumeroLikes);

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

        public async Task<bool> UpdateAsync(LikesDiarios likesDiarios)
        {
            if (likesDiarios == null)
                throw new ArgumentNullException(nameof(likesDiarios));

            const string query = @"
                UPDATE likes_diarios 
                SET fecha = @Fecha, 
                    id_perfil = @IdPerfil, 
                    numero_likes = @NumeroLikes 
                WHERE id_likes_diarios = @Id";
            
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Fecha", likesDiarios.Fecha);
                command.Parameters.AddWithValue("@IdPerfil", likesDiarios.IdPerfil);
                command.Parameters.AddWithValue("@NumeroLikes", likesDiarios.NumeroLikes);
                command.Parameters.AddWithValue("@Id", likesDiarios.IdLikesDiarios);

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
            const string query = "DELETE FROM likes_diarios WHERE id_likes_diarios = @Id";
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

        public async Task<LikesDiarios?> GetLikesDiariosByPerfilAsync(int idPerfil)
        {
            const string query = @"
                SELECT ld.*, p.nombre as perfil_nombre 
                FROM likes_diarios ld 
                INNER JOIN perfil p ON ld.id_perfil = p.id_perfil 
                WHERE ld.id_perfil = @IdPerfil 
                AND ld.fecha = CURDATE()";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@IdPerfil", idPerfil);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new LikesDiarios
                {
                    IdLikesDiarios = Convert.ToInt32(reader["id_likes_diarios"]),
                    Fecha = Convert.ToDateTime(reader["fecha"]),
                    IdPerfil = Convert.ToInt32(reader["id_perfil"]),
                    NumeroLikes = Convert.ToInt32(reader["numero_likes"])
                };
            }

            return null;
        }
    }
} 