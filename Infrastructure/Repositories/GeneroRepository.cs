using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using CampusLove.Infrastructure.Configuration;
using CampusLove2.Domain.Entities;

namespace CampusLove.Infrastructure.Repositories
{
    public class GeneroRepository
    {
        private readonly MySqlConnection _connection;

        public GeneroRepository()
        {
            _connection = DatabaseConfig.GetConnection();
        }

        public async Task<IEnumerable<Genero>> GetAllAsync()
        {
            var generoList = new List<Genero>();
            const string query = "SELECT id_genero, descripcion FROM genero";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                generoList.Add(new Genero
                {
                    IdGenero = Convert.ToInt32(reader["id_genero"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty
                });
            }

            return generoList;
        }

        public async Task<Genero?> GetByIdAsync(int id)
        {
            const string query = "SELECT id_genero, descripcion FROM genero WHERE id_genero = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Genero
                {
                    IdGenero = Convert.ToInt32(reader["id_genero"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Genero genero)
        {
            if (genero == null)
                throw new ArgumentNullException(nameof(genero));

            const string query = "INSERT INTO genero (descripcion) VALUES (@Descripcion)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Descripcion", genero.Descripcion);

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

        public async Task<bool> UpdateAsync(Genero genero)
        {
            if (genero == null)
                throw new ArgumentNullException(nameof(genero));

            const string query = "UPDATE genero SET descripcion = @Descripcion WHERE id_genero = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Descripcion", genero.Descripcion);
                command.Parameters.AddWithValue("@Id", genero.IdGenero);

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
            const string query = "DELETE FROM genero WHERE id_genero = @Id";
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