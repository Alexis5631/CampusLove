using System;
using System.Collections.Generic;
using CampusLove.Infrastructure.Repositories;

namespace CampusLove2.Aplication.UI
{
    public class MenuSignUp
    {
        private readonly UsuarioRepository _usuarioRepository;

        public MenuSignUp()
        {
            _usuarioRepository = new UsuarioRepository();
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
    }
}