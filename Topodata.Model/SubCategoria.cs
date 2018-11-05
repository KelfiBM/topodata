using System;
using System.Collections.Generic;

namespace Topodata.Model
{
    public class SubCategoria
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string ImagePath { get; set; }
        public DateTime FechaRegistro { get; set; }
        public List<Categoria> Categorias { get; set; }
    }
}
