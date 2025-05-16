using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove.Domain.Entities
{
    public class UsuariosIntereses
    {
        public int IdUserInteres { get; set; }
        public int IdUsuarios { get; set; } // Relación con Usuarios
        public int IdIntereses { get; set; } // Relación con Intereses
    }
}