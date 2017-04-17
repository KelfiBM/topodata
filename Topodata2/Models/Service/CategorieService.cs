using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Topodata2.Models.Entities;

namespace Topodata2.Models.Service
{
    public class CategorieService
    {
        private readonly TopodataContext _db = new TopodataContext();

        public List<Categoria> GetCategorias()
        {
            var result = _db.Categorias.ToList();
            return result;
        }

        
    }
}