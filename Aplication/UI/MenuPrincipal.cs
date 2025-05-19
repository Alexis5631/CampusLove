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
        private readonly MenuLogin _menuLogin;
        private Usuarios? _usuarioActual;

        public MenuPrincipal()
        {
            _connection = DatabaseConfig.GetConnection();
            _menuSignUp = new MenuSignUp();
            _menuLogin = new MenuLogin();
        }

        public async Task MostrarMenu()
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                if (_usuarioActual == null)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
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
                            _usuarioActual = await _menuLogin.ValidateUser();
                            if (_usuarioActual != null)
                            {
                                await _menuLogin.ShowMenu(_usuarioActual);
                                _usuarioActual = null; // Resetear usuario al cerrar sesión
                            }
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
        }
    }
}