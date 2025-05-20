using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove2.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using CampusLove.Infrastructure.Configuration;
using MySql.Data.MySqlClient;

namespace CampusLove2.Aplication.UI
{
    public class MenuOpciones
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly PerfilRepository _perfilRepository;
        private readonly InteresesRepository _interesesRepository;
        private readonly ProfesionRepository _profesionRepository;
        private readonly CiudadRepository _ciudadRepository;
        
        public MenuOpciones()
        {
            _usuarioRepository = new UsuarioRepository();
            _perfilRepository = new PerfilRepository();
            _interesesRepository = new InteresesRepository();
            _profesionRepository = new ProfesionRepository();
            _ciudadRepository = new CiudadRepository();
        }
        
        public async Task ShowConfigMenu(Usuarios currentUser)
        {
            bool returnToMainMenu = false;
            
            while (!returnToMainMenu)
            {
                Console.Clear();
                Console.WriteLine("=== MENÚ DE CONFIGURACIÓN ===\n");
                Console.WriteLine("1. Cambiar contraseña");
                Console.WriteLine("2. Cambiar ciudad");
                Console.WriteLine("3. Cambiar intereses");
                Console.WriteLine("4. Cambiar biografía");
                Console.WriteLine("5. Cambiar profesión");
                Console.WriteLine("6. Cambiar edad");
                Console.WriteLine("7. Volver al menú principal");
                
                string option = ReadText("\nSeleccione una opción: ");
                
                try
                {
                    switch (option)
                    {
                        case "1":
                            await CambiarPassword(currentUser);
                            break;
                        case "2":
                            await CambiarCiudad(currentUser);
                            break;
                        case "3":
                            await CambiarIntereses(currentUser);
                            break;
                        case "4":
                            await CambiarBiografia(currentUser);
                            break;
                        case "5":
                            await CambiarProfesion(currentUser);
                            break;
                        case "6":
                            await CambiarEdad(currentUser);
                            break;
                        case "7":
                            returnToMainMenu = true;
                            break;
                        default:
                            ShowMessage("Opción inválida. Por favor intente de nuevo.", ConsoleColor.Red);
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error: {ex.Message}", ConsoleColor.Red);
                    Console.ReadKey();
                }
            }
        }
        
        private async Task CambiarPassword(Usuarios currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== CAMBIAR CONTRASEÑA ===\n");
            
            string currentPassword = ReadText("Ingrese su contraseña actual: ");
            
            if (currentPassword != currentUser.Password)
            {
                ShowMessage("La contraseña actual es incorrecta.", ConsoleColor.Red);
                Console.ReadKey();
                return;
            }
            
            string newPassword = ReadText("Ingrese su nueva contraseña: ");
            string confirmPassword = ReadText("Confirme su nueva contraseña: ");
            
            if (newPassword != confirmPassword)
            {
                ShowMessage("Las contraseñas no coinciden.", ConsoleColor.Red);
                Console.ReadKey();
                return;
            }
            
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ShowMessage("La contraseña no puede estar vacía.", ConsoleColor.Red);
                Console.ReadKey();
                return;
            }
            
            // Actualizar la contraseña
            currentUser.Password = newPassword;
            bool result = await _usuarioRepository.UpdateAsync(currentUser);
            
            if (result)
            {
                ShowMessage("Contraseña actualizada exitosamente.", ConsoleColor.Green);
            }
            else
            {
                ShowMessage("Error al actualizar la contraseña.", ConsoleColor.Red);
            }
            
            Console.ReadKey();
        }
        
        private async Task CambiarCiudad(Usuarios currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== CAMBIAR CIUDAD ===\n");
            
            // Obtener el perfil actual del usuario
            var perfil = await _perfilRepository.GetByIdAsync(currentUser.IdPerfil);
            if (perfil == null)
            {
                ShowMessage("No se pudo obtener el perfil del usuario.", ConsoleColor.Red);
                Console.ReadKey();
                return;
            }
            
            // Mostrar la ciudad actual
            var ciudadActual = await _ciudadRepository.GetByIdAsync(perfil.IdCiudad);
            if (ciudadActual != null)
            {
                Console.WriteLine($"Ciudad actual: {ciudadActual.Nombre}");
            }
            
            // Mostrar lista de ciudades disponibles
            Console.WriteLine("\nCiudades disponibles:");
            var ciudades = await _ciudadRepository.GetAllAsync();
            foreach (var ciudad in ciudades)
            {
                Console.WriteLine($"{ciudad.IdCiudad}. {ciudad.Nombre}");
            }
            
            // Opción para agregar nueva ciudad
            Console.WriteLine("0. Agregar nueva ciudad");
            
            int idCiudad = ReadInt("\nSeleccione una ciudad (0 para agregar nueva): ");
            
            if (idCiudad == 0)
            {
                // Agregar nueva ciudad
                string nombreCiudad = ReadText("Ingrese el nombre de la nueva ciudad: ");
                
                if (string.IsNullOrWhiteSpace(nombreCiudad))
                {
                    ShowMessage("El nombre de la ciudad no puede estar vacío.", ConsoleColor.Red);
                    Console.ReadKey();
                    return;
                }
                
                // Mostrar regiones disponibles
                Console.WriteLine("\nRegiones disponibles:");
                var regiones = _usuarioRepository.ObtenerOpciones("region");
                foreach (var region in regiones)
                {
                    Console.WriteLine($"{region.Key}. {region.Value}");
                }
                
                int idRegion = ReadInt("\nSeleccione una región: ");
                
                // Registrar nueva ciudad
                idCiudad = _usuarioRepository.RegistrarNuevaOpcion("ciudad", nombreCiudad, idRegion);
            }
            
            // Actualizar la ciudad del perfil
            perfil.IdCiudad = idCiudad;
            bool result = await _perfilRepository.UpdateAsync(perfil);
            
            if (result)
            {
                ShowMessage("Ciudad actualizada exitosamente.", ConsoleColor.Green);
            }
            else
            {
                ShowMessage("Error al actualizar la ciudad.", ConsoleColor.Red);
            }
            
            Console.ReadKey();
        }
        
        private async Task CambiarIntereses(Usuarios currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== CAMBIAR INTERESES ===\n");
            
            // Mostrar intereses actuales del usuario
            Console.WriteLine("Intereses actuales:");
            var interesesUsuario = await _interesesRepository.GetInteresesByUsuarioAsync(currentUser.IdPerfil);
            if (interesesUsuario.Any())
            {
                foreach (var interes in interesesUsuario)
                {
                    Console.WriteLine($"- {interes.Descripcion}");
                }
            }
            else
            {
                Console.WriteLine("No tienes intereses registrados actualmente.");
            }
            
            // Mostrar todos los intereses disponibles
            Console.WriteLine("\nIntereses disponibles:");
            var todosIntereses = await _interesesRepository.GetAllAsync();
            foreach (var interes in todosIntereses)
            {
                Console.WriteLine($"{interes.IdIntereses}. {interes.Descripcion}");
            }
            
            // Opción para agregar nuevo interés
            Console.WriteLine("0. Agregar nuevo interés");
            
            List<int> nuevosIntereses = new List<int>();
            bool seguirAgregando = true;
            
            while (seguirAgregando)
            {
                int idInteres = ReadInt("\nSeleccione un interés (0 para agregar nuevo, -1 para terminar): ");
                
                if (idInteres == -1)
                {
                    seguirAgregando = false;
                }
                else if (idInteres == 0)
                {
                    // Agregar nuevo interés
                    string descripcionInteres = ReadText("Ingrese la descripción del nuevo interés: ");
                    
                    if (string.IsNullOrWhiteSpace(descripcionInteres))
                    {
                        ShowMessage("La descripción no puede estar vacía.", ConsoleColor.Red);
                        continue;
                    }
                    
                    // Registrar nuevo interés
                    idInteres = _usuarioRepository.RegistrarNuevaOpcion("intereses", descripcionInteres);
                    nuevosIntereses.Add(idInteres);
                }
                else if (todosIntereses.Any(i => i.IdIntereses == idInteres))
                {
                    nuevosIntereses.Add(idInteres);
                }
                else
                {
                    ShowMessage("Interés no válido.", ConsoleColor.Red);
                }
            }
            
            // Actualizar intereses del usuario
            if (nuevosIntereses.Count > 0)
            {
                // Eliminar intereses actuales
                await _interesesRepository.DeleteInteresesByUsuarioAsync(currentUser.IdPerfil);
                
                // Agregar nuevos intereses
                foreach (var idInteres in nuevosIntereses)
                {
                    await _interesesRepository.AddInteresUsuarioAsync(currentUser.IdPerfil, idInteres);
                }
                
                ShowMessage("Intereses actualizados exitosamente.", ConsoleColor.Green);
            }
            else
            {
                ShowMessage("No se seleccionaron nuevos intereses.", ConsoleColor.Yellow);
            }
            
            Console.ReadKey();
        }
        
        private async Task CambiarBiografia(Usuarios currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== CAMBIAR BIOGRAFÍA ===\n");
            
            // Obtener el perfil actual del usuario
            var perfil = await _perfilRepository.GetByIdAsync(currentUser.IdPerfil);
            if (perfil == null)
            {
                ShowMessage("No se pudo obtener el perfil del usuario.", ConsoleColor.Red);
                Console.ReadKey();
                return;
            }
            
            // Mostrar biografía actual
            Console.WriteLine($"Biografía actual: {perfil.Biografia}");
            
            // Solicitar nueva biografía
            string nuevaBiografia = ReadText("\nIngrese su nueva biografía: ");
            
            if (string.IsNullOrWhiteSpace(nuevaBiografia))
            {
                ShowMessage("La biografía no puede estar vacía.", ConsoleColor.Red);
                Console.ReadKey();
                return;
            }
            
            // Actualizar biografía
            perfil.Biografia = nuevaBiografia;
            bool result = await _perfilRepository.UpdateAsync(perfil);
            
            if (result)
            {
                ShowMessage("Biografía actualizada exitosamente.", ConsoleColor.Green);
            }
            else
            {
                ShowMessage("Error al actualizar la biografía.", ConsoleColor.Red);
            }
            
            Console.ReadKey();
        }
        
        private async Task CambiarProfesion(Usuarios currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== CAMBIAR PROFESIÓN ===\n");
            
            // Obtener el perfil actual del usuario
            var perfil = await _perfilRepository.GetByIdAsync(currentUser.IdPerfil);
            if (perfil == null)
            {
                ShowMessage("No se pudo obtener el perfil del usuario.", ConsoleColor.Red);
                Console.ReadKey();
                return;
            }
            
            // Mostrar profesión actual
            var profesionActual = await _profesionRepository.GetByIdAsync(perfil.IdProfesion);
            if (profesionActual != null)
            {
                Console.WriteLine($"Profesión actual: {profesionActual.Descripcion}");
            }
            
            // Mostrar lista de profesiones disponibles
            Console.WriteLine("\nProfesiones disponibles:");
            var profesiones = await _profesionRepository.GetAllAsync();
            foreach (var profesion in profesiones)
            {
                Console.WriteLine($"{profesion.IdProfesion}. {profesion.Descripcion}");
            }
            
            // Opción para agregar nueva profesión
            Console.WriteLine("0. Agregar nueva profesión");
            
            int idProfesion = ReadInt("\nSeleccione una profesión (0 para agregar nueva): ");
            
            if (idProfesion == 0)
            {
                // Agregar nueva profesión
                string descripcionProfesion = ReadText("Ingrese la descripción de la nueva profesión: ");
                
                if (string.IsNullOrWhiteSpace(descripcionProfesion))
                {
                    ShowMessage("La descripción no puede estar vacía.", ConsoleColor.Red);
                    Console.ReadKey();
                    return;
                }
                
                // Registrar nueva profesión
                idProfesion = _usuarioRepository.RegistrarNuevaOpcion("profesion", descripcionProfesion);
            }
            
            // Actualizar la profesión del perfil
            perfil.IdProfesion = idProfesion;
            bool result = await _perfilRepository.UpdateAsync(perfil);
            
            if (result)
            {
                ShowMessage("Profesión actualizada exitosamente.", ConsoleColor.Green);
            }
            else
            {
                ShowMessage("Error al actualizar la profesión.", ConsoleColor.Red);
            }
            
            Console.ReadKey();
        }
        
        private async Task CambiarEdad(Usuarios currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== CAMBIAR EDAD ===\n");
            
            // Obtener el perfil actual del usuario
            var perfil = await _perfilRepository.GetByIdAsync(currentUser.IdPerfil);
            if (perfil == null)
            {
                ShowMessage("No se pudo obtener el perfil del usuario.", ConsoleColor.Red);
                Console.ReadKey();
                return;
            }
            
            // Mostrar la edad actual
            Console.WriteLine($"Edad actual: {perfil.Edad}");
            
            // Solicitar la nueva edad
            int nuevaEdad = 0;
            Console.Write("\nIngrese su nueva edad: ");
            while (!int.TryParse(Console.ReadLine(), out nuevaEdad) || nuevaEdad < 18 || nuevaEdad > 100)
            {
                Console.WriteLine("Edad no válida. Debe ser un número entre 18 y 100.");
                Console.Write("Ingrese su nueva edad: ");
            }
            
            // Actualizar la edad del perfil
            perfil.Edad = nuevaEdad;
            bool result = await _perfilRepository.UpdateAsync(perfil);
            
            if (result)
            {
                ShowMessage("Edad actualizada exitosamente.", ConsoleColor.Green);
            }
            else
            {
                ShowMessage("Error al actualizar la edad.", ConsoleColor.Red);
            }
            
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