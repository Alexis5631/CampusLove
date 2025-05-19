using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using CampusLove.Infrastructure.Configuration;
using CampusLove2.Domain.Entities;

namespace CampusLove.Infrastructure.Repositories
{
    public class UsuarioRepository
    {
        private readonly MySqlConnection _connection;

        public UsuarioRepository()
        {
            _connection = DatabaseConfig.GetConnection();
        }

        public async Task<IEnumerable<Usuarios>> GetAllAsync()
        {
            var usuariosList = new List<Usuarios>();
            const string query = @"
                SELECT u.*, p.nombre, p.apellido, p.identificacion, p.biografia 
                FROM usuarios u 
                INNER JOIN perfil p ON u.id_perfil = p.id_perfil";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                usuariosList.Add(new Usuarios
                {
                    IdUsuarios = Convert.ToInt32(reader["id_usuarios"]),
                    Username = reader["username"].ToString() ?? string.Empty,
                    Password = reader["password"].ToString() ?? string.Empty,
                    FechaNacimiento = Convert.ToDateTime(reader["fecha_nacimiento"]),
                    IdPerfil = Convert.ToInt32(reader["id_perfil"])
                });
            }

            return usuariosList;
        }

        public async Task<Usuarios?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT u.*, p.nombre, p.apellido, p.identificacion, p.biografia 
                FROM usuarios u 
                INNER JOIN perfil p ON u.id_perfil = p.id_perfil 
                WHERE u.id_usuarios = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Usuarios
                {
                    IdUsuarios = Convert.ToInt32(reader["id_usuarios"]),
                    Username = reader["username"].ToString() ?? string.Empty,
                    Password = reader["password"].ToString() ?? string.Empty,
                    FechaNacimiento = Convert.ToDateTime(reader["fecha_nacimiento"]),
                    IdPerfil = Convert.ToInt32(reader["id_perfil"])
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Usuarios usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            const string query = @"
                INSERT INTO usuarios (username, password, fecha_nacimiento, id_perfil) 
                VALUES (@Username, @Password, @FechaNacimiento, @IdPerfil)";
            
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Username", usuario.Username);
                command.Parameters.AddWithValue("@Password", usuario.Password);
                command.Parameters.AddWithValue("@FechaNacimiento", usuario.FechaNacimiento);
                command.Parameters.AddWithValue("@IdPerfil", usuario.IdPerfil);

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

        public async Task<bool> UpdateAsync(Usuarios usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            const string query = @"
                UPDATE usuarios 
                SET username = @Username, 
                    password = @Password, 
                    fecha_nacimiento = @FechaNacimiento, 
                    id_perfil = @IdPerfil 
                WHERE id_usuarios = @Id";

            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Username", usuario.Username);
                command.Parameters.AddWithValue("@Password", usuario.Password);
                command.Parameters.AddWithValue("@FechaNacimiento", usuario.FechaNacimiento);
                command.Parameters.AddWithValue("@IdPerfil", usuario.IdPerfil);
                command.Parameters.AddWithValue("@Id", usuario.IdUsuarios);

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
            const string query = "DELETE FROM usuarios WHERE id_usuarios = @Id";
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

        public bool VerificarNicknameExistente(string nickname)
        {
            const string query = "SELECT COUNT(*) FROM usuarios WHERE username = @Nickname";
            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Nickname", nickname);
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        public int RegistrarPerfil(string nombre, string apellido, string identificacion, string biografia, 
            int idGenero, int idEstado, int idProfesion, int idCiudad)
        {
            string insertPerfilQuery = @"
                INSERT INTO perfil (nombre, apellido, identificacion, biografia, total_likes, id_genero, id_estado, id_profesion, id_ciudad) 
                VALUES (@nombre, @apellido, @identificacion, @biografia, 0, @idGenero, @idEstado, @idProfesion, @idCiudad)";
            
            using (var cmd = new MySqlCommand(insertPerfilQuery, _connection))
            {
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@apellido", apellido);
                cmd.Parameters.AddWithValue("@identificacion", identificacion);
                cmd.Parameters.AddWithValue("@biografia", biografia);
                cmd.Parameters.AddWithValue("@idGenero", idGenero);
                cmd.Parameters.AddWithValue("@idEstado", idEstado);
                cmd.Parameters.AddWithValue("@idProfesion", idProfesion);
                cmd.Parameters.AddWithValue("@idCiudad", idCiudad);
                
                cmd.ExecuteNonQuery();
                return (int)cmd.LastInsertedId;
            }
        }

        public void RegistrarUsuario(string nickname, string password, int idPerfil)
        {
            string insertUsuarioQuery = @"
                INSERT INTO usuarios (username, password, fecha_nacimiento, id_perfil) 
                VALUES (@username, @password, @fechaNacimiento, @idPerfil)";
            
            using (var cmd = new MySqlCommand(insertUsuarioQuery, _connection))
            {
                cmd.Parameters.AddWithValue("@username", nickname);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@fechaNacimiento", DateTime.Now);
                cmd.Parameters.AddWithValue("@idPerfil", idPerfil);
                cmd.ExecuteNonQuery();
            }
        }

        public void EliminarPerfil(int idPerfil)
        {
            string deletePerfilQuery = "DELETE FROM perfil WHERE id_perfil = @idPerfil";
            using (var cmd = new MySqlCommand(deletePerfilQuery, _connection))
            {
                cmd.Parameters.AddWithValue("@idPerfil", idPerfil);
                cmd.ExecuteNonQuery();
            }
        }

        public Dictionary<int, string> ObtenerOpciones(string tabla, int? idRegion = null)
        {
            string idCol = "id_" + tabla;
            string descCol = tabla switch
            {
                "ciudad" or "region" => "Nombre",
                "pais" => "nombre",
                _ => "descripcion"
            };

            string query = $"SELECT {idCol}, {descCol} FROM {tabla}";
            if (tabla == "ciudad" && idRegion.HasValue)
            {
                query += $" WHERE id_region = {idRegion.Value}";
            }

            var opciones = new Dictionary<int, string>();
            using var cmd = new MySqlCommand(query, _connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                opciones.Add(Convert.ToInt32(reader[idCol]), reader[descCol].ToString() ?? string.Empty);
            }

            return opciones;
        }

        public int RegistrarNuevaOpcion(string tabla, string valor, int? idRegion = null, int? idPais = null)
        {
            string idCol = "id_" + tabla;
            string descCol = tabla switch
            {
                "ciudad" or "region" => "Nombre",
                "pais" => "nombre",
                _ => "descripcion"
            };

            string query = $"INSERT INTO {tabla} ({descCol}";
            if (tabla == "ciudad" && idRegion.HasValue)
            {
                query += ", id_region) VALUES (@valor, @idRegion)";
            }
            else if (tabla == "region" && idPais.HasValue)
            {
                query += ", id_pais) VALUES (@valor, @idPais)";
            }
            else
            {
                query += ") VALUES (@valor)";
            }

            using var cmd = new MySqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@valor", valor);
            if (tabla == "ciudad" && idRegion.HasValue)
            {
                cmd.Parameters.AddWithValue("@idRegion", idRegion.Value);
            }
            else if (tabla == "region" && idPais.HasValue)
            {
                cmd.Parameters.AddWithValue("@idPais", idPais.Value);
            }

            cmd.ExecuteNonQuery();
            return (int)cmd.LastInsertedId;
        }

        public async Task<Usuarios?> GetByUsernameAsync(string username)
        {
            const string query = @"
                SELECT u.*, p.nombre, p.apellido, p.identificacion, p.biografia 
                FROM usuarios u 
                INNER JOIN perfil p ON u.id_perfil = p.id_perfil 
                WHERE u.username = @Username";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Usuarios
                {
                    IdUsuarios = Convert.ToInt32(reader["id_usuarios"]),
                    Username = reader["username"].ToString() ?? string.Empty,
                    Password = reader["password"].ToString() ?? string.Empty,
                    FechaNacimiento = Convert.ToDateTime(reader["fecha_nacimiento"]),
                    IdPerfil = Convert.ToInt32(reader["id_perfil"])
                };
            }

            return null;
        }

        public async Task<Usuarios?> GetByPerfilIdAsync(int perfilId)
        {
            const string query = @"
                SELECT u.*, p.nombre, p.apellido, p.identificacion, p.biografia 
                FROM usuarios u 
                INNER JOIN perfil p ON u.id_perfil = p.id_perfil 
                WHERE u.id_perfil = @PerfilId";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@PerfilId", perfilId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Usuarios
                {
                    IdUsuarios = Convert.ToInt32(reader["id_usuarios"]),
                    Username = reader["username"].ToString() ?? string.Empty,
                    Password = reader["password"].ToString() ?? string.Empty,
                    FechaNacimiento = Convert.ToDateTime(reader["fecha_nacimiento"]),
                    IdPerfil = Convert.ToInt32(reader["id_perfil"])
                };
            }

            return null;
        }
    }
}