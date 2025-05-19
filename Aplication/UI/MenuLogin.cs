using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove2.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using CampusLove.Infrastructure.Configuration;
using MySql.Data.MySqlClient;
using CampusLove2.Aplication.UI;

namespace CampusLove2.Aplication.UI
{
    public class MenuLogin
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly PerfilRepository _perfilRepository;
        private readonly ReaccionesRepository _reaccionesRepository;
        private readonly UserMatchRepository _userMatchRepository;
        private readonly MenuSignUp _menuSignUp;

        public MenuLogin()
        {
            _usuarioRepository = new UsuarioRepository();
            _perfilRepository = new PerfilRepository();
            _reaccionesRepository = new ReaccionesRepository();
            _userMatchRepository = new UserMatchRepository();
            _menuSignUp = new MenuSignUp();
        }

        public async Task<Usuarios?> ValidateUser()
        {
            Console.Clear();
            Console.WriteLine("=== INICIO DE SESIÓN ===");
            Console.WriteLine("Bienvenido a CampusLove");
            Console.WriteLine("------------------");

            try
            {
                string username = ReadText("\nUsuario: ").Trim();
                if (string.IsNullOrWhiteSpace(username))
                {
                    ShowMessage("El usuario no puede estar vacío.", ConsoleColor.Red);
                    return null;
                }

                string password = ReadText("Contraseña: ").Trim();
                if (string.IsNullOrWhiteSpace(password))
                {
                    ShowMessage("La contraseña no puede estar vacía.", ConsoleColor.Red);
                    return null;
                }

                var usuario = await _usuarioRepository.GetByUsernameAsync(username);
                if (usuario == null)
                {
                    ShowMessage("El usuario no existe.", ConsoleColor.Red);
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                    return null;
                }

                if (usuario.Password != password)
                {
                    ShowMessage("Contraseña incorrecta.", ConsoleColor.Red);
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                    return null;
                }

                ShowMessage("\n¡Inicio de sesión exitoso!", ConsoleColor.Green);
                Console.WriteLine($"Bienvenido, {usuario.Username}!");
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
                return usuario;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al iniciar sesión: {ex.Message}", ConsoleColor.Red);
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
                return null;
            }
        }

        public async Task ShowMenu(Usuarios currentUser)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool returnToMain = false;

            while (!returnToMain)
            {
                Console.Clear();
                Console.WriteLine($"=== MENÚ DE USUARIO === - {currentUser.Username}");

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("=== MENÚ DE USUARIO ===");
                Console.WriteLine("1. Ver Perfiles");
                Console.WriteLine("2. Interactuar con Perfiles");
                Console.WriteLine("3. Ver Matches");
                Console.WriteLine("4. Configuración ");
                Console.WriteLine("5. Cerrar Sesión");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Gray;
                string option = ReadText("\nSeleccione una opción: ");

                try
                {
                    switch (option)
                    {
                        case "1":
                            await ViewProfiles(currentUser);
                            break;
                        case "2":
                            await InteractWithProfiles(currentUser);
                            break;
                        case "3":
                            await ViewMatches(currentUser);
                            break;
                        case "4":
                            // TODO: Implementar menú de configuración
                            break;    
                        case "5":
                            returnToMain = true;
                            ShowMessage("\nCerrando sesión...", ConsoleColor.Blue);
                            break;
                        default:
                            ShowMessage("Opción inválida. Por favor intente de nuevo.", ConsoleColor.Red);
                            Console.ReadKey();
                            break;  
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage($"\nError: {ex.Message}", ConsoleColor.Red);
                    Console.ReadKey();
                }
            }
        }

        private async Task ViewProfiles(Usuarios currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== PERFILES DISPONIBLES ===");
            Console.WriteLine("----------------------");

            var perfiles = await _perfilRepository.GetAllAsync();
            foreach (var perfil in perfiles)
            {
                if (perfil.IdPerfil != currentUser.IdPerfil)
                {
                    Console.WriteLine($"\nID: {perfil.IdPerfil}");
                    Console.WriteLine($"Nombre: {perfil.Nombre} {perfil.Apellido}");
                    Console.WriteLine($"Biografía: {perfil.Biografia}");
                    Console.WriteLine($"Total Likes: {perfil.TotalLikes}");
                    Console.WriteLine("----------------------");
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPresione cualquier tecla para continuar...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task InteractWithProfiles(Usuarios currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== INTERACTUAR CON PERFILES ===");
            Console.WriteLine("------------------------");

            var perfiles = await _perfilRepository.GetAllAsync();
            var perfilesDisponibles = perfiles.Where(p => p.IdPerfil != currentUser.IdPerfil).ToList();

            if (!perfilesDisponibles.Any())
            {
                ShowMessage("No hay perfiles disponibles para interactuar.", ConsoleColor.Yellow);
                return;
            }

            foreach (var perfil in perfilesDisponibles)
            {
                Console.WriteLine($"\nID: {perfil.IdPerfil}");
                Console.WriteLine($"Nombre: {perfil.Nombre} {perfil.Apellido}");
                Console.WriteLine($"Biografía: {perfil.Biografia}");
                Console.WriteLine($"Total Likes: {perfil.TotalLikes}");
                Console.WriteLine("----------------------");
            }

            int perfilId = ReadInt("\nIngrese el ID del perfil para interactuar (0 para salir): ");
            if (perfilId == 0) return;

            var perfilSeleccionado = perfilesDisponibles.FirstOrDefault(p => p.IdPerfil == perfilId);
            if (perfilSeleccionado == null)
            {
                ShowMessage("Perfil no encontrado.", ConsoleColor.Red);
                return;
            }

            Console.WriteLine("\n1. Like ❤️");
            Console.WriteLine("2. Dislike 👎");
            int opcion = ReadInt("\nSeleccione una opción: ");

            var reaccion = new Reacciones
            {
                IdUsuarios = currentUser.IdUsuarios,
                IdPerfil = perfilSeleccionado.IdPerfil,
                Tipo = opcion == 1 ? "Like" : "Dislike",
                FechaReaccion = DateTime.Now
            };

            await _reaccionesRepository.InsertAsync(reaccion);
            ShowMessage("Reacción registrada exitosamente.", ConsoleColor.Green);
            
            // Recargar el perfil para mostrar los likes actualizados
            if (reaccion.Tipo == "Like")
            {
                var perfilActualizado = await _perfilRepository.GetByIdAsync(perfilSeleccionado.IdPerfil);
                if (perfilActualizado != null)
                {
                    Console.WriteLine($"\nPerfil actualizado: {perfilActualizado.Nombre} {perfilActualizado.Apellido}");
                    Console.WriteLine($"Total Likes: {perfilActualizado.TotalLikes}");
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPresione cualquier tecla para continuar...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task ViewMatches(Usuarios currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== MIS MATCHES ===");
            Console.WriteLine("-------------");

            var matches = await _userMatchRepository.GetMatchesByUsuarioAsync(currentUser.IdUsuarios);
            if (!matches.Any())
            {
                ShowMessage("No tienes matches aún.", ConsoleColor.Yellow);
                return;
            }

            foreach (var match in matches)
            {
                var otroUsuario = match.IdUser1 == currentUser.IdUsuarios ? match.IdUser2 : match.IdUser1;
                var perfil = await _perfilRepository.GetByIdAsync(otroUsuario);
                
                if (perfil != null)
                {
                    Console.WriteLine($"\nMatch con: {perfil.Nombre} {perfil.Apellido}");
                    Console.WriteLine($"Fecha del match: {match.FechaMatch}");
                    Console.WriteLine("----------------------");
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPresione cualquier tecla para continuar...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static string ReadText(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? string.Empty;
        }

        private static int ReadInt(string prompt)
        {
            Console.Write(prompt);
            return int.TryParse(Console.ReadLine(), out int result) ? result : 0;
        }

        private static void ShowMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}