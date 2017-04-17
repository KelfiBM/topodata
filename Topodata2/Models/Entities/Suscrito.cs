namespace Topodata2.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Suscrito")]
    public partial class Suscrito
    {
        [Key]
        public int IdSuscrito { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        public bool Informed { get; set; }
    }
}
