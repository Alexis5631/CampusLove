using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove2.Domain.Entities
{
    public class LikesDiarios
    {
        public int IdLikesDiarios { get; set; }
        public DateTime Fecha { get; set; }
        public int IdPerfil { get; set; } // Relación con Perfil
        public int NumeroLikes { get; set; }
    }
}