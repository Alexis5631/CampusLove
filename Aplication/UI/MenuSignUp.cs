using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using CampusLove.Infrastructure.Configuration;

namespace CampusLove2.Aplication.UI
{
    public class MenuSignUp
    {
        private readonly MySqlConnection _connection;

        public MenuSignUp()
        {
            _connection = DatabaseConfig.GetConnection();
        }

        public void MostrarMenuRegistro()
        {
            Console.Clear();
            Console.WriteLine("=== REGISTRO DE USUARIO ===");

            Console.Write("Ingrese su nombre: ");
            string nombre = Console.ReadLine();

            Console.Write("Ingrese su apellido: ");
            string apellido = Console.ReadLine();

            Console.Write("Ingrese su identificación: ");
            string identificacion = Console.ReadLine();

            Console.Write("Ingrese su biografía: ");
            string biografia = Console.ReadLine();

            // Selección de género
            Console.WriteLine("\nSeleccione su género:");
            Console.WriteLine("1. Masculino");
            Console.WriteLine("2. Femenino");
            Console.WriteLine("3. No binario");
            Console.WriteLine("4. Prefiero no decirlo");
            Console.Write("Seleccione el número de su género: ");
            int idGenero;
            while (!int.TryParse(Console.ReadLine(), out idGenero) || idGenero < 1 || idGenero > 4)
            {
                Console.WriteLine("Opción no válida. Por favor seleccione un número entre 1 y 4.");
                Console.Write("Seleccione el número de su género: ");
            }

            // Selección de estado
            int idEstado = SeleccionarOpcionDesdeTabla("estado");
            if (idEstado == -1) return;

            // Selección de profesión
            int idProfesion = SeleccionarOpcionDesdeTablaConNuevaOpcion("profesion");
            if (idProfesion == -1) return;

            // Selección de región
            int idRegion = SeleccionarOpcionDesdeTablaConNuevaOpcion("region");
            if (idRegion == -1) return;

            // Selección de ciudad
            int idCiudad = SeleccionarOpcionDesdeTablaConNuevaOpcion("ciudad", idRegion);
            if (idCiudad == -1) return;

            Console.Write("\nIngrese su nickname: ");
            string nickname = Console.ReadLine();

            // Verificar si el usuario ya existe
            string checkQuery = "SELECT COUNT(*) FROM usuarios WHERE username = @username";
            using (var cmd = new MySqlCommand(checkQuery, _connection))
            {
                cmd.Parameters.AddWithValue("@username", nickname);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count > 0)
                {
                    Console.WriteLine("\nEste nickname ya está registrado. Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    return;
                }
            }

            Console.Write("Ingrese su contraseña: ");
            string password = Console.ReadLine();

            // Insertar en perfil
            string insertPerfilQuery = @"
                INSERT INTO perfil (nombre, apellido, identificacion, biografia, total_likes, id_genero, id_estado, id_profesion, id_ciudad) 
                VALUES (@nombre, @apellido, @identificacion, @biografia, 0, @idGenero, @idEstado, @idProfesion, @idCiudad)";
            int idPerfil = 0;
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
                try
                {
                    cmd.ExecuteNonQuery();
                    idPerfil = (int)cmd.LastInsertedId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError al registrar perfil: {ex.Message}");
                    Console.ReadKey();
                    return;
                }
            }

            // Insertar en usuarios
            string insertUsuarioQuery = @"
                INSERT INTO usuarios (username, password, fecha_nacimiento, id_perfil) 
                VALUES (@username, @password, @fechaNacimiento, @idPerfil)";
            using (var cmd = new MySqlCommand(insertUsuarioQuery, _connection))
            {
                cmd.Parameters.AddWithValue("@username", nickname);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@fechaNacimiento", DateTime.Now);
                cmd.Parameters.AddWithValue("@idPerfil", idPerfil);
                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("\n¡Registro exitoso! Presione cualquier tecla para continuar...");
                }
                catch (Exception ex)
                {
                    string deletePerfilQuery = "DELETE FROM perfil WHERE id_perfil = @idPerfil";
                    using (var deleteCmd = new MySqlCommand(deletePerfilQuery, _connection))
                    {
                        deleteCmd.Parameters.AddWithValue("@idPerfil", idPerfil);
                        deleteCmd.ExecuteNonQuery();
                    }
                    Console.WriteLine($"\nError al registrar usuario: {ex.Message}");
                }
            }
            Console.ReadKey();
        }

        private int SeleccionarOpcionDesdeTabla(string tabla)
        {
            string idCol = "id_" + tabla;
            string descCol;
            switch (tabla)
            {
                case "ciudad":
                    descCol = "Nombre";
                    break;
                case "region":
                    descCol = "Nombre";
                    break;
                case "pais":
                    descCol = "nombre";
                    break;
                default:
                    descCol = "descripcion";
                    break;
            }
            string query = $"SELECT {idCol}, {descCol} FROM {tabla}";
            var opciones = new Dictionary<int, string>();
            using (var cmd = new MySqlCommand(query, _connection))
            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine($"\nSeleccione {tabla}:");
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader[idCol]);
                    string desc = reader[descCol].ToString();
                    opciones[id] = desc;
                    Console.WriteLine($"{id}. {desc}");
                }
            }
            Console.Write($"Ingrese el número de {tabla}: ");
            int seleccion;
            while (!int.TryParse(Console.ReadLine(), out seleccion) || !opciones.ContainsKey(seleccion))
            {
                Console.WriteLine("Opción no válida. Intente de nuevo.");
                Console.Write($"Ingrese el número de {tabla}: ");
            }
            return seleccion;
        }

        private int SeleccionarOpcionDesdeTablaConNuevaOpcion(string tabla, int? idRegion = null)
        {
            string idCol = "id_" + tabla;
            string descCol;
            switch (tabla)
            {
                case "ciudad":
                    descCol = "Nombre";
                    break;
                case "region":
                    descCol = "Nombre";
                    break;
                case "pais":
                    descCol = "nombre";
                    break;
                default:
                    descCol = "descripcion";
                    break;
            }

            string query = $"SELECT {idCol}, {descCol} FROM {tabla}";
            if (tabla == "ciudad" && idRegion.HasValue)
            {
                query += $" WHERE id_region = {idRegion.Value}";
            }
            var opciones = new Dictionary<int, string>();
            using (var cmd = new MySqlCommand(query, _connection))
            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine($"\nSeleccione {tabla}:");
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader[idCol]);
                    string desc = reader[descCol].ToString();
                    opciones[id] = desc;
                    Console.WriteLine($"{id}. {desc}");
                }
            }
            Console.WriteLine($"0. Escribir una nueva {tabla}");
            Console.Write($"Ingrese el número de {tabla} o (0) para agregar una nueva: ");
            int seleccion;
            while (!int.TryParse(Console.ReadLine(), out seleccion) || (!opciones.ContainsKey(seleccion) && seleccion != 0))
            {
                Console.WriteLine("Opción no válida. Intente de nuevo.");
                Console.Write($"Ingrese el número de {tabla} o (0) para agregar una nueva: ");
            }
            if (seleccion == 0)
            {
                Console.Write($"Ingrese el nombre de la nueva {tabla}: ");
                string nuevoValor = Console.ReadLine();
                // Insertar nueva opción en la base de datos
                string insertQuery;
                if (tabla == "ciudad")
                {
                    insertQuery = "INSERT INTO ciudad (Nombre, id_region) VALUES (@nombre, @idRegion)";
                    using (var cmd = new MySqlCommand(insertQuery, _connection))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nuevoValor);
                        cmd.Parameters.AddWithValue("@idRegion", idRegion.Value);
                        cmd.ExecuteNonQuery();
                        return (int)cmd.LastInsertedId;
                    }
                }
                else if (tabla == "region")
                {
                    // Primero mostrar países disponibles
                    Console.WriteLine("\nSeleccione el país al que pertenece la región:");
                    int idPais = SeleccionarOpcionDesdeTabla("pais");
                    insertQuery = "INSERT INTO region (Nombre, id_pais) VALUES (@nombre, @idPais)";
                    using (var cmd = new MySqlCommand(insertQuery, _connection))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nuevoValor);
                        cmd.Parameters.AddWithValue("@idPais", idPais);
                        cmd.ExecuteNonQuery();
                        return (int)cmd.LastInsertedId;
                    }
                }
                else
                {
                    insertQuery = "INSERT INTO profesion (descripcion) VALUES (@descripcion)";
                    using (var cmd = new MySqlCommand(insertQuery, _connection))
                    {
                        cmd.Parameters.AddWithValue("@descripcion", nuevoValor);
                        cmd.ExecuteNonQuery();
                        return (int)cmd.LastInsertedId;
                    }
                }
            }
            return seleccion;
        }
    }
}