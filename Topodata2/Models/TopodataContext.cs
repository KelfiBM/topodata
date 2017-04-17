namespace Topodata2.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Entities;

    public partial class TopodataContext : DbContext
    {
        public TopodataContext()
            : base("name=TopodataContext")
        {
            Database.SetInitializer<TopodataContext>(null);
        }

        public virtual DbSet<Categoria> Categorias { get; set; }
        public virtual DbSet<Contenido> Contenidoes { get; set; }
        public virtual DbSet<Documento> Documentoes { get; set; }
        public virtual DbSet<Flipboard> Flipboards { get; set; }
        public virtual DbSet<HomeSlideImageSeason> HomeSlideImageSeasons { get; set; }
        public virtual DbSet<HomeSlideVideo> HomeSlideVideos { get; set; }
        public virtual DbSet<OurTeam> OurTeams { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SubCategoria> SubCategorias { get; set; }
        public virtual DbSet<Suscrito> Suscritoes { get; set; }
        public virtual DbSet<TextoHome> TextoHomes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<HomeSlideData> HomeSlideDatas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categoria>()
                .HasMany(e => e.SubCategorias)
                .WithMany(e => e.Categorias)
                .Map(m => m.ToTable("Categoria_SubCategoria").MapLeftKey("IdCategoria").MapRightKey("IdSubCategoria"));

            modelBuilder.Entity<Contenido>()
                .HasMany(e => e.Documentoes)
                .WithRequired(e => e.Contenido)
                .HasForeignKey(e => e.IdContenido)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Contenido>()
                .HasMany(e => e.SubCategorias)
                .WithMany(e => e.Contenidoes)
                .Map(m => m.ToTable("SubCategoria_Contenido").MapLeftKey("IdContenido").MapRightKey("IdSubCategoria"));

            modelBuilder.Entity<HomeSlideImageSeason>()
                .HasMany(e => e.HomeSlideDatas)
                .WithRequired(e => e.HomeSlideImageSeason)
                .HasForeignKey(e => e.idHomeSlideImageSeason)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HomeSlideVideo>()
                .HasMany(e => e.HomeSlideDatas)
                .WithRequired(e => e.HomeSlideVideo)
                .HasForeignKey(e => e.idHomeSlideVideo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.Role)
                .HasForeignKey(e => e.IdRole)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SubCategoria>()
                .HasMany(e => e.Documentoes)
                .WithRequired(e => e.SubCategoria)
                .HasForeignKey(e => e.IdSubCategoria)
                .WillCascadeOnDelete(false);
        }
    }
}
