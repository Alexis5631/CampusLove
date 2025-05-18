using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using CampusLove.Infrastructure.Configuration;
using CampusLove2.Domain.Entities;

namespace CampusLove.Infrastructure.Repositories
{
    public class EstadoRepository
    {
        private readonly MySqlConnection _connection;

        public EstadoRepository()
        {
            _connection = DatabaseConfig.GetConnection();
        }

        public async Task<IEnumerable<Estado>> GetAllAsync()
        {
            var estadoList = new List<Estado>();
            const string query = "SELECT id_estado, descripcion FROM estado";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                estadoList.Add(new Estado
                {
                    IdEstado = Convert.ToInt32(reader["id_estado"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty
                });
            }

            return estadoList;
        }

        public async Task<Estado?> GetByIdAsync(int id)
        {
            const string query = "SELECT id_estado, descripcion FROM estado WHERE id_estado = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Estado
                {
                    IdEstado = Convert.ToInt32(reader["id_estado"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Estado estado)
        {
            if (estado == null)
                throw new ArgumentNullException(nameof(estado));

            const string query = "INSERT INTO estado (descripcion) VALUES (@Descripcion)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Descripcion", estado.Descripcion);

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

        public async Task<bool> UpdateAsync(Estado estado)
        {
            if (estado == null)
                throw new ArgumentNullException(nameof(estado));

            const string query = "UPDATE estado SET descripcion = @Descripcion WHERE id_estado = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Descripcion", estado.Descripcion);
                command.Parameters.AddWithValue("@Id", estado.IdEstado);

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
            const string query = "DELETE FROM estado WHERE id_estado = @Id";
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