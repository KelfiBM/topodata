namespace Topodata2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TextoHome")]
    public partial class TextoHome
    {
        [Key]
        public int idTextoHome { get; set; }

        [Required]
        [StringLength(200)]
        public string Agrimensura { get; set; }

        [Required]
        [StringLength(200)]
        public string EstudioSuelo { get; set; }

        [Required]
        [StringLength(200)]
        public string Diseno { get; set; }

        [Required]
        [StringLength(200)]
        public string Ingenieria { get; set; }

        public DateTime regDate { get; set; }
    }
}
