using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove2.Domain.Entities
{
    public class Usuarios
    {
        public int IdUsuarios { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int IdPerfil { get; set; } // Relación con Perfil
        
        // Propiedades de navegación
        public Perfil? Perfil { get; set; }
    }
}