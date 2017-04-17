namespace Topodata2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HomeSlideImageSeason")]
    public partial class HomeSlideImageSeason
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HomeSlideImageSeason()
        {
            HomeSlideDatas = new HashSet<HomeSlideData>();
        }

        public int Id { get; set; }

        [Required]
        public string ImagePath { get; set; }

        public DateTime? RegDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HomeSlideData> HomeSlideDatas { get; set; }
    }
}
