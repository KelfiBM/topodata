using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topodata.Model
{
    public class TipoContenido
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public List<SubCategoria> SubCategorias { get; set; }
    }
}
