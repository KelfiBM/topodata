namespace Topodata2.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HomeSlideVideo")]
    public partial class HomeSlideVideo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HomeSlideVideo()
        {
            HomeSlideDatas = new HashSet<HomeSlideData>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string UrlVideo { get; set; }

        public DateTime regDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HomeSlideData> HomeSlideDatas { get; set; }
    }
}
