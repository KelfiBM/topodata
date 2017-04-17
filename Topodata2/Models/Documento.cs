namespace Topodata2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Documento")]
    public partial class Documento
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Nombre { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public string ImagePath { get; set; }

        [Required]
        public string Url { get; set; }

        public DateTime RegDate { get; set; }

        public int IdSubCategoria { get; set; }

        public int IdContenido { get; set; }

        public string DescripcionHtml { get; set; }

        public virtual Contenido Contenido { get; set; }

        public virtual SubCategoria SubCategoria { get; set; }
    }
}
