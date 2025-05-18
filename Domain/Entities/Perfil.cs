using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove2.Domain.Entities
{
    public class Perfil
    {
        public int IdPerfil { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Identificacion { get; set; }
        public string? Biografia { get; set; }
        public int TotalLikes { get; set; }
        public int IdCiudad { get; set; } // Relación con Ciudad
        public int IdGenero { get; set; } // Relación con Genero
        public int IdEstado { get; set; } // Relación con Estado
        public int IdProfesion { get; set; } // Relación con Profesion
    }
}