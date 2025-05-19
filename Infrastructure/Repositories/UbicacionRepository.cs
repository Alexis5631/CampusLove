using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using CampusLove.Infrastructure.Configuration;
using CampusLove.Domain.Entities;

namespace CampusLove.Infrastructure.Repositories
{
    public class UbicacionRepository
    {
        private readonly MySqlConnection _connection;

        public UbicacionRepository()
        {
            _connection = DatabaseConfig.GetConnection();
        }

        // Métodos para Region
        public async Task<IEnumerable<Region>> GetAllRegionesAsync()
        {
            var regionList = new List<Region>();
            const string query = @"
                SELECT r.*, p.nombre as pais_nombre 
                FROM region r 
                INNER JOIN pais p ON r.id_pais = p.id_pais";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                regionList.Add(new Region
                {
                    IdRegion = Convert.ToInt32(reader["id_region"]),
                    Nombre = reader["Nombre"].ToString() ?? string.Empty,
                    IdPais = Convert.ToInt32(reader["id_pais"])
                });
            }

            return regionList;
        }

        public async Task<Region?> GetRegionByIdAsync(int id)
        {
            const string query = @"
                SELECT r.*, p.nombre as pais_nombre 
                FROM region r 
                INNER JOIN pais p ON r.id_pais = p.id_pais 
                WHERE r.id_region = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Region
                {
                    IdRegion = Convert.ToInt32(reader["id_region"]),
                    Nombre = reader["Nombre"].ToString() ?? string.Empty,
                    IdPais = Convert.ToInt32(reader["id_pais"])
                };
            }

            return null;
        }

        public async Task<bool> InsertRegionAsync(Region region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            const string query = "INSERT INTO region (Nombre, id_pais) VALUES (@Nombre, @IdPais)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Nombre", region.Nombre);
                command.Parameters.AddWithValue("@IdPais", region.IdPais);

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

        public async Task<bool> UpdateRegionAsync(Region region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            const string query = "UPDATE region SET Nombre = @Nombre, id_pais = @IdPais WHERE id_region = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Nombre", region.Nombre);
                command.Parameters.AddWithValue("@IdPais", region.IdPais);
                command.Parameters.AddWithValue("@Id", region.IdRegion);

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

        public async Task<bool> DeleteRegionAsync(int id)
        {
            const string query = "DELETE FROM region WHERE id_region = @Id";
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

        // Métodos para Ciudad
        public async Task<IEnumerable<Ciudad>> GetAllCiudadesAsync()
        {
            var ciudadList = new List<Ciudad>();
            const string query = @"
                SELECT c.*, r.Nombre as region_nombre, p.nombre as pais_nombre 
                FROM ciudad c 
                INNER JOIN region r ON c.id_region = r.id_region 
                INNER JOIN pais p ON r.id_pais = p.id_pais";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                ciudadList.Add(new Ciudad
                {
                    IdCiudad = Convert.ToInt32(reader["id_ciudad"]),
                    Nombre = reader["Nombre"].ToString() ?? string.Empty,
                    IdRegion = Convert.ToInt32(reader["id_region"])
                });
            }

            return ciudadList;
        }

        public async Task<Ciudad?> GetCiudadByIdAsync(int id)
        {
            const string query = @"
                SELECT c.*, r.Nombre as region_nombre, p.nombre as pais_nombre 
                FROM ciudad c 
                INNER JOIN region r ON c.id_region = r.id_region 
                INNER JOIN pais p ON r.id_pais = p.id_pais 
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
                    IdRegion = Convert.ToInt32(reader["id_region"])
                };
            }

            return null;
        }

        public async Task<bool> InsertCiudadAsync(Ciudad ciudad)
        {
            if (ciudad == null)
                throw new ArgumentNullException(nameof(ciudad));

            const string query = "INSERT INTO ciudad (Nombre, id_region) VALUES (@Nombre, @IdRegion)";
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

        public async Task<bool> UpdateCiudadAsync(Ciudad ciudad)
        {
            if (ciudad == null)
                throw new ArgumentNullException(nameof(ciudad));

            const string query = "UPDATE ciudad SET Nombre = @Nombre, id_region = @IdRegion WHERE id_ciudad = @Id";
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

        public async Task<bool> DeleteCiudadAsync(int id)
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

        // Métodos para Pais
        public async Task<IEnumerable<Pais>> GetAllPaisesAsync()
        {
            var paisList = new List<Pais>();
            const string query = "SELECT id_pais, nombre FROM pais";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                paisList.Add(new Pais
                {
                    IdPais = Convert.ToInt32(reader["id_pais"]),
                    Nombre = reader["nombre"].ToString() ?? string.Empty
                });
            }

            return paisList;
        }

        public async Task<Pais?> GetPaisByIdAsync(int id)
        {
            const string query = "SELECT id_pais, nombre FROM pais WHERE id_pais = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Pais
                {
                    IdPais = Convert.ToInt32(reader["id_pais"]),
                    Nombre = reader["nombre"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<bool> InsertPaisAsync(Pais pais)
        {
            if (pais == null)
                throw new ArgumentNullException(nameof(pais));

            const string query = "INSERT INTO pais (nombre) VALUES (@Nombre)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Nombre", pais.Nombre);

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

        public async Task<bool> UpdatePaisAsync(Pais pais)
        {
            if (pais == null)
                throw new ArgumentNullException(nameof(pais));

            const string query = "UPDATE pais SET nombre = @Nombre WHERE id_pais = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Nombre", pais.Nombre);
                command.Parameters.AddWithValue("@Id", pais.IdPais);

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

        public async Task<bool> DeletePaisAsync(int id)
        {
            const string query = "DELETE FROM pais WHERE id_pais = @Id";
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

        // Métodos adicionales
        public async Task<IEnumerable<Ciudad>> GetCiudadesByRegionAsync(int idRegion)
        {
            var ciudadList = new List<Ciudad>();
            const string query = @"
                SELECT c.*, r.Nombre as region_nombre 
                FROM ciudad c 
                INNER JOIN region r ON c.id_region = r.id_region 
                WHERE c.id_region = @IdRegion";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@IdRegion", idRegion);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ciudadList.Add(new Ciudad
                {
                    IdCiudad = Convert.ToInt32(reader["id_ciudad"]),
                    Nombre = reader["Nombre"].ToString() ?? string.Empty,
                    IdRegion = Convert.ToInt32(reader["id_region"])
                });
            }

            return ciudadList;
        }

        public async Task<IEnumerable<Region>> GetRegionesByPaisAsync(int idPais)
        {
            var regionList = new List<Region>();
            const string query = @"
                SELECT r.*, p.nombre as pais_nombre 
                FROM region r 
                INNER JOIN pais p ON r.id_pais = p.id_pais 
                WHERE r.id_pais = @IdPais";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@IdPais", idPais);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                regionList.Add(new Region
                {
                    IdRegion = Convert.ToInt32(reader["id_region"]),
                    Nombre = reader["Nombre"].ToString() ?? string.Empty,
                    IdPais = Convert.ToInt32(reader["id_pais"])
                });
            }

            return regionList;
        }
    }
}