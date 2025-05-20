using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using CampusLove.Infrastructure.Configuration;
using CampusLove2.Domain.Entities;

namespace CampusLove2.Aplication.UI
{
    public class MenuEstadisticas
    {
        private readonly MySqlConnection _connection;

        public MenuEstadisticas()
        {
            _connection = DatabaseConfig.GetConnection();
        }

        public async Task MostrarEstadisticas()
        {
            try
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("=== ESTADÍSTICAS DEL SISTEMA ===\n");
                
                await MostrarUsuariosConMasLikes();
                Console.WriteLine();
                await MostrarUsuariosConMasMatches();
                Console.WriteLine();
                await MostrarInteresesPopulares();
                
                Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
                Console.ResetColor();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError al mostrar estadísticas: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
                Console.ReadKey();
            }
        }
        
        private async Task MostrarUsuariosConMasLikes()
        {
            try
            {
                // Mostrar usuarios con más likes
                Console.WriteLine("USUARIOS CON MÁS LIKES:");
                Console.WriteLine("------------------------");
                
                string queryLikes = @"
                    SELECT p.id_perfil, p.nombre, p.apellido, p.total_likes
                    FROM perfil p
                    ORDER BY p.total_likes DESC
                    LIMIT 5";
                
                using (var cmd = new MySqlCommand(queryLikes, _connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        int rank = 1;
                        bool hayResultados = false;
                        
                        while (reader.Read())
                        {
                            hayResultados = true;
                            string nombre = reader["nombre"].ToString() ?? string.Empty;
                            string apellido = reader["apellido"].ToString() ?? string.Empty;
                            int likes = Convert.ToInt32(reader["total_likes"]);
                            
                            Console.WriteLine($"{rank}. {nombre} {apellido} - {likes} likes");
                            rank++;
                        }
                        
                        if (!hayResultados)
                        {
                            Console.WriteLine("No hay datos disponibles.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al mostrar usuarios con más likes: {ex.Message}");
            }
        }
        
        private async Task MostrarUsuariosConMasMatches()
        {
            try
            {
                // Mostrar usuarios con más matches
                Console.WriteLine("USUARIOS CON MÁS MATCHES:");
                Console.WriteLine("-------------------------");
                
                string queryMatches = @"
                    SELECT u.id_usuarios, p.nombre, p.apellido, COUNT(m.id_user_match) as total_matches
                    FROM usuarios u
                    JOIN perfil p ON u.id_perfil = p.id_perfil
                    LEFT JOIN user_match m ON u.id_usuarios = m.id_user1 OR u.id_usuarios = m.id_user2
                    GROUP BY u.id_usuarios, p.nombre, p.apellido
                    ORDER BY total_matches DESC
                    LIMIT 5";
                
                using (var cmd = new MySqlCommand(queryMatches, _connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        int rank = 1;
                        bool hayResultados = false;
                        
                        while (reader.Read())
                        {
                            hayResultados = true;
                            string nombre = reader["nombre"].ToString() ?? string.Empty;
                            string apellido = reader["apellido"].ToString() ?? string.Empty;
                            int matches = Convert.ToInt32(reader["total_matches"]);
                            
                            Console.WriteLine($"{rank}. {nombre} {apellido} - {matches} matches");
                            rank++;
                        }
                        
                        if (!hayResultados)
                        {
                            Console.WriteLine("No hay datos disponibles.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al mostrar usuarios con más matches: {ex.Message}");
            }
        }
        
        private async Task MostrarInteresesPopulares()
        {
            try
            {
                // Mostrar intereses más populares
                Console.WriteLine("INTERESES MÁS POPULARES:");
                Console.WriteLine("-------------------------");
                
                string queryIntereses = @"
                    SELECT i.descripcion, COUNT(ui.id_user_interes) as total
                    FROM intereses i
                    LEFT JOIN usuarios_intereses ui ON i.id_intereses = ui.id_intereses
                    GROUP BY i.id_intereses, i.descripcion
                    ORDER BY total DESC
                    LIMIT 5";
                
                using (var cmd = new MySqlCommand(queryIntereses, _connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        int rank = 1;
                        bool hayResultados = false;
                        
                        while (reader.Read())
                        {
                            hayResultados = true;
                            string descripcion = reader["descripcion"].ToString() ?? string.Empty;
                            int total = Convert.ToInt32(reader["total"]);
                            
                            Console.WriteLine($"{rank}. {descripcion} - {total} usuarios");
                            rank++;
                        }
                        
                        if (!hayResultados)
                        {
                            Console.WriteLine("No hay datos disponibles.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al mostrar intereses populares: {ex.Message}");
            }
        }
    }
}
