using System;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;
using CampusLove.Infrastructure.Configuration;
using CampusLove2.Aplication.UI;

namespace CampusLove2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Mostrar mensaje de bienvenida
                MostrarBienvenida();
                
                try
                {
                    // Iniciar la aplicación
                    MenuPrincipal menu = new MenuPrincipal();
                    await menu.MostrarMenu();
                }
                catch (MySqlException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nError de conexión a la base de datos: " + ex.Message);
                    Console.WriteLine("Por favor, verifique que el servidor MySQL esté en ejecución y que la configuración sea correcta.");
                    Console.ResetColor();
                    Console.WriteLine("\nPresione cualquier tecla para salir...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError inesperado: {ex.Message}");
                Console.WriteLine("Detalles: " + ex.StackTrace);
                Console.ResetColor();
                Console.WriteLine("\nPresione cualquier tecla para salir...");
                Console.ReadKey();
            }
        }
        
        private static void MostrarBienvenida()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(@"
            ╔═════════════════════════════════════════════════════════╗
            ║                                                         ║
            ║   ██████╗ █████╗ ███╗   ███╗██████╗ ██╗   ██╗███████╗   ║
            ║  ██╔════╝██╔══██╗████╗ ████║██╔══██╗██║   ██║██╔════╝   ║
            ║  ██║     ███████║██╔████╔██║██████╔╝██║   ██║███████╗   ║
            ║  ██║     ██╔══██║██║╚██╔╝██║██╔═══╝ ██║   ██║╚════██║   ║
            ║  ╚██████╗██║  ██║██║ ╚═╝ ██║██║     ╚██████╔╝███████║   ║
            ║   ╚═════╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝      ╚═════╝ ╚══════╝   ║
            ║                                                         ║
            ║                      ██╗      ██████╗ ██╗   ██╗███████╗ ║
            ║                      ██║     ██╔═══██╗██║   ██║██╔════╝ ║
            ║                      ██║     ██║   ██║██║   ██║█████╗   ║
            ║                      ██║     ██║   ██║╚██╗ ██╔╝██╔══╝   ║
            ║                      ███████╗╚██████╔╝ ╚████╔╝ ███████╗ ║
            ║                      ╚══════╝ ╚═════╝   ╚═══╝  ╚══════╝ ║
            ║                                                         ║
            ╚═════════════════════════════════════════════════════════╝
            ");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n\t\t¡Bienvenido a la aplicación de citas para estudiantes!");
            Console.ResetColor();
            Console.WriteLine("\nCargando aplicación...");
            System.Threading.Thread.Sleep(1500); // Pausa para efecto visual
            Console.Clear();
        }
        

    }
}