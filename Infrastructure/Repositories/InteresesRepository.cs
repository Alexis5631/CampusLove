using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using CampusLove.Infrastructure.Configuration;
using CampusLove.Domain.Entities;

namespace CampusLove.Infrastructure.Repositories
{
    public class InteresesRepository
    {
        private readonly MySqlConnection _connection;

        public InteresesRepository()
        {
            _connection = DatabaseConfig.GetConnection();
        }

        public async Task<IEnumerable<Intereses>> GetAllAsync()
        {
            var interesesList = new List<Intereses>();
            const string query = "SELECT id_intereses, descripcion FROM intereses";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                interesesList.Add(new Intereses
                {
                    IdIntereses = Convert.ToInt32(reader["id_intereses"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty
                });
            }

            return interesesList;
        }

        public async Task<Intereses?> GetByIdAsync(int id)
        {
            const string query = "SELECT id_intereses, descripcion FROM intereses WHERE id_intereses = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Intereses
                {
                    IdIntereses = Convert.ToInt32(reader["id_intereses"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Intereses interes)
        {
            if (interes == null)
                throw new ArgumentNullException(nameof(interes));

            const string query = "INSERT INTO intereses (descripcion) VALUES (@Descripcion)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Descripcion", interes.Descripcion);

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

        public async Task<bool> UpdateAsync(Intereses interes)
        {
            if (interes == null)
                throw new ArgumentNullException(nameof(interes));

            const string query = "UPDATE intereses SET descripcion = @Descripcion WHERE id_intereses = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Descripcion", interes.Descripcion);
                command.Parameters.AddWithValue("@Id", interes.IdIntereses);

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
            const string query = "DELETE FROM intereses WHERE id_intereses = @Id";
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

        public async Task<IEnumerable<Intereses>> GetInteresesByUsuarioAsync(int idUsuario)
        {
            var interesesList = new List<Intereses>();
            const string query = @"
                SELECT i.* 
                FROM intereses i 
                INNER JOIN usuarios_intereses ui ON i.id_intereses = ui.id_intereses 
                WHERE ui.id_usuarios = @IdUsuario";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@IdUsuario", idUsuario);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                interesesList.Add(new Intereses
                {
                    IdIntereses = Convert.ToInt32(reader["id_intereses"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty
                });
            }

            return interesesList;
        }
    }
}