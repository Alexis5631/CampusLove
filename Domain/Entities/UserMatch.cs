using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove2.Domain.Entities
{
    public class UserMatch
    {
        public int Id { get; set; }
        public int IdUser1 { get; set; } // Relación con Usuarios
        public int IdUser2 { get; set; } // Relación con Usuarios
        public DateTime FechaMatch { get; set; }
    }
}