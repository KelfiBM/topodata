namespace Topodata2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Flipboard")]
    public partial class Flipboard
    {
        [Key]
        public int IdFlipboard { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }

        [Required]
        public string Url { get; set; }

        public DateTime regDate { get; set; }
    }
}
