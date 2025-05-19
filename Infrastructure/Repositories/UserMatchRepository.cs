using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using CampusLove.Infrastructure.Configuration;
using CampusLove2.Domain.Entities;

namespace CampusLove.Infrastructure.Repositories
{
    public class UserMatchRepository
    {
        private readonly MySqlConnection _connection;

        public UserMatchRepository()
        {
            _connection = DatabaseConfig.GetConnection();
        }

        public async Task<IEnumerable<UserMatch>> GetAllAsync()
        {
            var userMatches = new List<UserMatch>();
            const string query = @"
                SELECT um.*, 
                       u1.username as usuario1_nombre,
                       u2.username as usuario2_nombre
                FROM user_match um
                INNER JOIN usuarios u1 ON um.id_user1 = u1.id_usuarios
                INNER JOIN usuarios u2 ON um.id_user2 = u2.id_usuarios";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                userMatches.Add(new UserMatch
                {
                    Id = Convert.ToInt32(reader["id_user_match"]),
                    IdUser1 = Convert.ToInt32(reader["id_user1"]),
                    IdUser2 = Convert.ToInt32(reader["id_user2"]),
                    FechaMatch = Convert.ToDateTime(reader["fecha_match"])
                });
            }

            return userMatches;
        }

        public async Task<UserMatch?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT um.*, 
                       u1.username as usuario1_nombre,
                       u2.username as usuario2_nombre
                FROM user_match um
                INNER JOIN usuarios u1 ON um.id_user1 = u1.id_usuarios
                INNER JOIN usuarios u2 ON um.id_user2 = u2.id_usuarios
                WHERE um.id_user_match = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UserMatch
                {
                    Id = Convert.ToInt32(reader["id_user_match"]),
                    IdUser1 = Convert.ToInt32(reader["id_user1"]),
                    IdUser2 = Convert.ToInt32(reader["id_user2"]),
                    FechaMatch = Convert.ToDateTime(reader["fecha_match"])
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(UserMatch userMatch)
        {
            if (userMatch == null)
                throw new ArgumentNullException(nameof(userMatch));

            const string query = @"
                INSERT INTO user_match (id_user1, id_user2, fecha_match) 
                VALUES (@IdUser1, @IdUser2, @FechaMatch)";
            
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@IdUser1", userMatch.IdUser1);
                command.Parameters.AddWithValue("@IdUser2", userMatch.IdUser2);
                command.Parameters.AddWithValue("@FechaMatch", DateTime.Now);

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

        public async Task<bool> UpdateAsync(UserMatch userMatch)
        {
            if (userMatch == null)
                throw new ArgumentNullException(nameof(userMatch));

            const string query = @"
                UPDATE user_match 
                SET id_user1 = @IdUser1, 
                    id_user2 = @IdUser2,
                    fecha_match = @FechaMatch
                WHERE id_user_match = @Id";
            
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@IdUser1", userMatch.IdUser1);
                command.Parameters.AddWithValue("@IdUser2", userMatch.IdUser2);
                command.Parameters.AddWithValue("@FechaMatch", userMatch.FechaMatch);
                command.Parameters.AddWithValue("@Id", userMatch.Id);

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
            const string query = "DELETE FROM user_match WHERE id_user_match = @Id";
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

        public async Task<IEnumerable<UserMatch>> GetMatchesByUsuarioAsync(int idUsuario)
        {
            var userMatches = new List<UserMatch>();
            const string query = @"
                SELECT um.*, 
                       u1.username as usuario1_nombre,
                       u2.username as usuario2_nombre
                FROM user_match um
                INNER JOIN usuarios u1 ON um.id_user1 = u1.id_usuarios
                INNER JOIN usuarios u2 ON um.id_user2 = u2.id_usuarios
                WHERE um.id_user1 = @IdUsuario OR um.id_user2 = @IdUsuario";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@IdUsuario", idUsuario);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                userMatches.Add(new UserMatch
                {
                    Id = Convert.ToInt32(reader["id_user_match"]),
                    IdUser1 = Convert.ToInt32(reader["id_user1"]),
                    IdUser2 = Convert.ToInt32(reader["id_user2"]),
                    FechaMatch = Convert.ToDateTime(reader["fecha_match"])
                });
            }

            return userMatches;
        }
    }
}