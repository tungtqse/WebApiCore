using EntityFramework.DbContextScope.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using WebApiCore.DataModel.Models;

namespace WebApiCore.DataAccess
{
    public class ApplicationDbContextOptions
    {
        public readonly DbContextOptions<MainContext> Options;

        public ApplicationDbContextOptions(DbContextOptions<MainContext> options)
        {
            Options = options;
        }
    }

    public partial class MainContext : DbContext, IDbContext
    {
        public MainContext() { }

        public MainContext(ApplicationDbContextOptions options) : base(options.Options) { }

        public virtual DbSet<AuditTrail> AuditTrails { get; set; }
        public virtual DbSet<AttachmentFile> AttachmentFiles { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Studio> Studios { get; set; }
        public virtual DbSet<Idol> Idols { get; set; }
        public virtual DbSet<Blood> Bloods { get; set; }
        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<MappingAlbumIdol> MappingAlbumIdols { get; set; }
        public virtual DbSet<MappingAlbumImage> MappingAlbumImages { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<MappingMovieCategory> MappingMovieCategories { get; set; }
        public virtual DbSet<MappingMovieIdol> MappingMovieIdols { get; set; }
        public virtual DbSet<MappingMovieImage> MappingMovieImages { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<MappingUserComment> MappingUserComments { get; set; }
        public virtual DbSet<SourceLink> SourceLinks { get; set; }
        public virtual DbSet<UserCredential> UserCredentials { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<MappingUserRole> MappingUserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); 
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.Relational().TableName = entityType.DisplayName();
            }

            base.OnModelCreating(modelBuilder);            

            modelBuilder.ApplyConfiguration(new MappingAlbumIdolConfiguration());
            modelBuilder.ApplyConfiguration(new MappingAlbumImageConfiguration());
            modelBuilder.ApplyConfiguration(new ImageConfiguration());
            modelBuilder.ApplyConfiguration(new AlbumConfiguration());
            modelBuilder.ApplyConfiguration(new IdolConfiguration());
            modelBuilder.ApplyConfiguration(new BloodConfiguration());
            modelBuilder.ApplyConfiguration(new MappingMovieCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new MappingMovieIdolConfiguration());
            modelBuilder.ApplyConfiguration(new MovieConfiguration());
            modelBuilder.ApplyConfiguration(new SourceLinkConfiguration());
            modelBuilder.ApplyConfiguration(new MappingUserCommentConfiguration());
            modelBuilder.ApplyConfiguration(new UserProfileConfiguration());
            modelBuilder.ApplyConfiguration(new UserCredentialConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new MappingUserRoleConfiguration());
        }
    }
}
