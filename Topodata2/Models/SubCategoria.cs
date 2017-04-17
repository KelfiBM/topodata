using Topodata2.Models.Entities;

namespace Topodata2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SubCategoria")]
    public partial class SubCategoria
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SubCategoria()
        {
            Documentoes = new HashSet<Documento>();
            Categorias = new HashSet<Categoria>();
            Contenidoes = new HashSet<Contenido>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Descripcion { get; set; }

        [Required]
        public string ImagePath { get; set; }

        public DateTime? RegDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Documento> Documentoes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Categoria> Categorias { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Contenido> Contenidoes { get; set; }
    }
}
