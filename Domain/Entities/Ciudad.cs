using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove.Domain.Entities
{
    public class Ciudad
    {
        public int IdCiudad { get; set; }
        public string? Nombre { get; set; }
        public int IdRegion { get; set; } // Relación con Region
    }
}