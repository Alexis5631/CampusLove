using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Infrastructure.Repositories;
using CampusLove.Domain.Entities;

namespace CampusLove2.Aplication.UI
{
    public class MenuSignUp
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly InteresesRepository _interesesRepository;

        public MenuSignUp()
        {
            _usuarioRepository = new UsuarioRepository();
            _interesesRepository = new InteresesRepository();
        }

        public async Task MostrarMenuRegistro()
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
            int idEstado = SeleccionarOpcion("estado");
            if (idEstado == -1) return;

            // Selección de profesión
            int idProfesion = SeleccionarOpcionConNueva("profesion");
            if (idProfesion == -1) return;

            // Selección de región
            int idRegion = SeleccionarOpcionConNueva("region");
            if (idRegion == -1) return;

            // Selección de ciudad
            int idCiudad = SeleccionarOpcionConNueva("ciudad", idRegion);
            if (idCiudad == -1) return;

            Console.Write("\nIngrese su nickname: ");
            string nickname = Console.ReadLine();

            if (_usuarioRepository.VerificarNicknameExistente(nickname))
            {
                Console.WriteLine("\nEste nickname ya está registrado. Presione cualquier tecla para continuar...");
                Console.ReadKey();
                return;
            }

            Console.Write("Ingrese su contraseña: ");
            string password = Console.ReadLine();

            try
            {
                int idPerfil = _usuarioRepository.RegistrarPerfil(nombre, apellido, identificacion, biografia, 
                    idGenero, idEstado, idProfesion, idCiudad);

                _usuarioRepository.RegistrarUsuario(nickname, password, idPerfil);
                
                // Selección de intereses
                Console.WriteLine("\n=== SELECCIÓN DE INTERESES ===");
                Console.WriteLine("Seleccione sus intereses (puede elegir varios):");
                List<int> interesesSeleccionados = SeleccionarIntereses();
                
                // Registrar intereses del usuario
                foreach (var idInteres in interesesSeleccionados)
                {
                    await _interesesRepository.AddInteresUsuarioAsync(idPerfil, idInteres);
                }
                
                Console.WriteLine("\n¡Registro exitoso! Presione cualquier tecla para continuar...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError al registrar usuario: {ex.Message}");
            }
            Console.ReadKey();
        }

        private int SeleccionarOpcion(string tabla)
        {
            var opciones = _usuarioRepository.ObtenerOpciones(tabla);
            Console.WriteLine($"\nSeleccione {tabla}:");
            foreach (var opcion in opciones)
            {
                Console.WriteLine($"{opcion.Key}. {opcion.Value}");
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

        private int SeleccionarOpcionConNueva(string tabla, int? idRegion = null)
        {
            var opciones = _usuarioRepository.ObtenerOpciones(tabla, idRegion);
            Console.WriteLine($"\nSeleccione {tabla}:");
            foreach (var opcion in opciones)
            {
                Console.WriteLine($"{opcion.Key}. {opcion.Value}");
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

                if (tabla == "region")
                {
                    Console.WriteLine("\nSeleccione el país al que pertenece la región:");
                    int idPais = SeleccionarOpcion("pais");
                    return _usuarioRepository.RegistrarNuevaOpcion(tabla, nuevoValor, null, idPais);
                }
                else
                {
                    return _usuarioRepository.RegistrarNuevaOpcion(tabla, nuevoValor, idRegion);
                }
            }
            return seleccion;
        }
        
        private List<int> SeleccionarIntereses()
        {
            List<int> interesesSeleccionados = new List<int>();
            bool seguirSeleccionando = true;
            
            // Obtener todos los intereses disponibles
            var intereses = _interesesRepository.GetAllAsync().Result.ToList();
            
            // Mostrar todos los intereses disponibles
            Console.WriteLine("\nIntereses disponibles:");
            foreach (var interes in intereses)
            {
                Console.WriteLine($"{interes.IdIntereses}. {interes.Descripcion}");
            }
            
            Console.WriteLine("0. Agregar nuevo interés");
            Console.WriteLine("-1. Terminar selección");
            
            while (seguirSeleccionando)
            {
                int idInteres = ReadInt("\nSeleccione un interés (0 para agregar nuevo, -1 para terminar): ");
                
                if (idInteres == -1)
                {
                    seguirSeleccionando = false;
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
                    
                    // Crear y guardar el nuevo interés
                    var nuevoInteres = new Intereses { Descripcion = descripcionInteres };
                    bool resultado = _interesesRepository.InsertAsync(nuevoInteres).Result;
                    
                    if (resultado)
                    {
                        // Obtener el ID del interés recién creado
                        var interesCreado = _interesesRepository.GetAllAsync().Result
                            .FirstOrDefault(i => i.Descripcion == descripcionInteres);
                        
                        if (interesCreado != null)
                        {
                            interesesSeleccionados.Add(interesCreado.IdIntereses);
                            ShowMessage($"Interés '{descripcionInteres}' agregado correctamente.", ConsoleColor.Green);
                            
                            // Actualizar la lista de intereses
                            intereses = _interesesRepository.GetAllAsync().Result.ToList();
                        }
                    }
                    else
                    {
                        ShowMessage("Error al agregar el nuevo interés.", ConsoleColor.Red);
                    }
                }
                else if (intereses.Any(i => i.IdIntereses == idInteres))
                {
                    // Verificar si ya está seleccionado
                    if (interesesSeleccionados.Contains(idInteres))
                    {
                        ShowMessage("Este interés ya ha sido seleccionado.", ConsoleColor.Yellow);
                    }
                    else
                    {
                        interesesSeleccionados.Add(idInteres);
                        var interesSeleccionado = intereses.First(i => i.IdIntereses == idInteres);
                        ShowMessage($"Interés '{interesSeleccionado.Descripcion}' seleccionado.", ConsoleColor.Green);
                    }
                }
                else
                {
                    ShowMessage("Interés no válido.", ConsoleColor.Red);
                }
            }
            
            return interesesSeleccionados;
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