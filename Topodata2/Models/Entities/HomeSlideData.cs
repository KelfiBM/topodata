namespace Topodata2.Models.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("HomeSlideData")]
    public class HomeSlideData
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int idHomeSlideVideo { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int idHomeSlideImageSeason { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime regDate { get; set; }

        public virtual HomeSlideImageSeason HomeSlideImageSeason { get; set; }

        public virtual HomeSlideVideo HomeSlideVideo { get; set; }
    }
}
