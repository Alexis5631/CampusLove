using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using CampusLove.Infrastructure.Configuration;
using CampusLove2.Domain.Entities;

namespace CampusLove.Infrastructure.Repositories
{
    public class ProfesionRepository
    {
        private readonly MySqlConnection _connection;

        public ProfesionRepository()
        {
            _connection = DatabaseConfig.GetConnection();
        }

        public async Task<IEnumerable<Profesion>> GetAllAsync()
        {
            var profesionList = new List<Profesion>();
            const string query = "SELECT id_profesion, descripcion FROM profesion";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                profesionList.Add(new Profesion
                {
                    IdProfesion = Convert.ToInt32(reader["id_profesion"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty
                });
            }

            return profesionList;
        }

        public async Task<Profesion?> GetByIdAsync(int id)
        {
            const string query = "SELECT id_profesion, descripcion FROM profesion WHERE id_profesion = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Profesion
                {
                    IdProfesion = Convert.ToInt32(reader["id_profesion"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Profesion profesion)
        {
            if (profesion == null)
                throw new ArgumentNullException(nameof(profesion));

            const string query = "INSERT INTO profesion (descripcion) VALUES (@Descripcion)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Descripcion", profesion.Descripcion);

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

        public async Task<bool> UpdateAsync(Profesion profesion)
        {
            if (profesion == null)
                throw new ArgumentNullException(nameof(profesion));

            const string query = "UPDATE profesion SET descripcion = @Descripcion WHERE id_profesion = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Descripcion", profesion.Descripcion);
                command.Parameters.AddWithValue("@Id", profesion.IdProfesion);

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
            const string query = "DELETE FROM profesion WHERE id_profesion = @Id";
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
    }
}