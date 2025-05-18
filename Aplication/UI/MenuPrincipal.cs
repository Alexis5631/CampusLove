using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using CampusLove.Infrastructure.Configuration;
using CampusLove2.Domain.Entities;

namespace CampusLove2.Aplication.UI
{
    public class MenuPrincipal
    {
        private readonly MySqlConnection _connection;
        private readonly MenuSignUp _menuSignUp;

        public MenuPrincipal()
        {
            _connection = DatabaseConfig.GetConnection();
            _menuSignUp = new MenuSignUp();
        }

        public void MostrarMenu()
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("=== MENÚ PRINCIPAL ===");
                Console.WriteLine("1. Registrarse");
                Console.WriteLine("2. Iniciar Sesión");
                Console.WriteLine("3. Salir");
                Console.Write("\nSeleccione una opción: ");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        _menuSignUp.MostrarMenuRegistro();
                        break;
                    case "2":
                        IniciarSesion();
                        break;
                    case "3":
                        salir = true;
                        Console.WriteLine("\n¡Gracias por usar el sistema!");
                        break;
                    default:
                        Console.WriteLine("\nOpción no válida. Presione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void IniciarSesion()
        {
            Console.Clear();
            Console.WriteLine("=== INICIO DE SESIÓN ===");
            
            Console.Write("Ingrese su nickname: ");
            string nickname = Console.ReadLine();

            Console.Write("Ingrese su contraseña: ");
            string password = Console.ReadLine();

            string query = "SELECT COUNT(*) FROM Usuarios WHERE Username = @username AND Password = @password";
            using (var cmd = new MySqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("@username", nickname);
                cmd.Parameters.AddWithValue("@password", password);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count > 0)
                {
                    Console.WriteLine("\n¡Inicio de sesión exitoso! Presione cualquier tecla para continuar...");
                }
                else
                {
                    Console.WriteLine("\nUsuario no encontrado o contraseña incorrecta. Por favor, regístrese primero.");
                    Console.WriteLine("Presione cualquier tecla para continuar...");
                }
            }
            Console.ReadKey();
        }
    }
}