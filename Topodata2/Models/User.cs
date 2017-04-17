namespace Topodata2.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        [Key]
        public int IdUsers { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public bool Informed { get; set; }

        public DateTime RegDate { get; set; }

        public int IdRole { get; set; }

        public virtual Role Role { get; set; }
    }
}
