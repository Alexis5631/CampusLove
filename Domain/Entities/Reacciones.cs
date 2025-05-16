using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove2.Domain.Entities
{
    public class Reacciones
    {
        public int IdReacciones { get; set; }
        public int IdUsuarios { get; set; } // Relación con Usuarios
        public int IdPerfil { get; set; } // Relación con Perfil
        public string? Tipo { get; set; } // 'Like' o 'Dislike'
        public DateTime FechaReaccion { get; set; }
    }
}