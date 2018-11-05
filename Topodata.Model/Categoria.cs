using System.Collections.Generic;

namespace Topodata.Model
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Icono { get; set; }

        public List<SubCategoria> SubCategorias { get; set; }
    }
}
