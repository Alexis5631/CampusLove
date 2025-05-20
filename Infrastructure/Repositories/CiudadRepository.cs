using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using CampusLove.Infrastructure.Configuration;
using CampusLove2.Domain.Entities;
using CampusLove.Domain.Entities;

namespace CampusLove.Infrastructure.Repositories
{
    public class CiudadRepository
    {
        private readonly MySqlConnection _connection;

        public CiudadRepository()
        {
            _connection = DatabaseConfig.GetConnection();
        }

        public async Task<IEnumerable<Ciudad>> GetAllAsync()
        {
            var ciudades = new List<Ciudad>();
            const string query = @"
                SELECT c.*, r.Nombre as region_nombre, p.nombre as pais_nombre
                FROM ciudad c
                LEFT JOIN region r ON c.id_region = r.id_region
                LEFT JOIN pais p ON r.id_pais = p.id_pais";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                ciudades.Add(new Ciudad
                {
                    IdCiudad = Convert.ToInt32(reader["id_ciudad"]),
                    Nombre = reader["Nombre"].ToString() ?? string.Empty,
                    IdRegion = reader["id_region"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_region"])
                });
            }

            return ciudades;
        }

        public async Task<Ciudad?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT c.*, r.Nombre as region_nombre, p.nombre as pais_nombre
                FROM ciudad c
                LEFT JOIN region r ON c.id_region = r.id_region
                LEFT JOIN pais p ON r.id_pais = p.id_pais
                WHERE c.id_ciudad = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Ciudad
                {
                    IdCiudad = Convert.ToInt32(reader["id_ciudad"]),
                    Nombre = reader["Nombre"].ToString() ?? string.Empty,
                    IdRegion = reader["id_region"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_region"])
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Ciudad ciudad)
        {
            if (ciudad == null)
                throw new ArgumentNullException(nameof(ciudad));

            const string query = @"
                INSERT INTO ciudad (Nombre, id_region) 
                VALUES (@Nombre, @IdRegion)";
            
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Nombre", ciudad.Nombre);
                command.Parameters.AddWithValue("@IdRegion", ciudad.IdRegion);

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

        public async Task<bool> UpdateAsync(Ciudad ciudad)
        {
            if (ciudad == null)
                throw new ArgumentNullException(nameof(ciudad));

            const string query = @"
                UPDATE ciudad 
                SET Nombre = @Nombre, 
                    id_region = @IdRegion
                WHERE id_ciudad = @Id";

            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Nombre", ciudad.Nombre);
                command.Parameters.AddWithValue("@IdRegion", ciudad.IdRegion);
                command.Parameters.AddWithValue("@Id", ciudad.IdCiudad);

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
            const string query = "DELETE FROM ciudad WHERE id_ciudad = @Id";
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
