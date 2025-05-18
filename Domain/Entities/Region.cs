using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove.Domain.Entities
{
    public class Region
    {
        public int IdRegion { get; set; }
        public string? Nombre { get; set; }
        public int IdPais { get; set; } // Relación con Pais
    }
}