using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using WebApiCore.DataModel.Models;

namespace WebApiCore.DataAccess
{
    public partial class MainContext
    {
        public class MappingAlbumIdolConfiguration : IEntityTypeConfiguration<MappingAlbumIdol>
        {
            public void Configure(EntityTypeBuilder<MappingAlbumIdol> builder)
            {
                builder.HasKey(c => new { c.AlbumId, c.IdolId });
                builder.HasOne<Album>(a => a.Album)
                        .WithMany(i => i.AlbumIdols)
                        .HasForeignKey(sc => sc.AlbumId);
                builder.HasOne<Idol>(a => a.Idol)
                        .WithMany(i => i.AlbumIdols)
                        .HasForeignKey(sc => sc.IdolId);

            }
        }

        public class MappingAlbumImageConfiguration : IEntityTypeConfiguration<MappingAlbumImage>
        {
            public void Configure(EntityTypeBuilder<MappingAlbumImage> builder)
            {
                builder.HasKey(c => new { c.AlbumId, c.ImageId });
                builder.HasOne<Album>(a => a.Album)
                        .WithMany(i => i.AlbumImages)
                        .HasForeignKey(sc => sc.AlbumId);
                builder.HasOne<Image>(a => a.Image)
                        .WithMany(i => i.AlbumImages)
                        .HasForeignKey(sc => sc.ImageId);

            }
        }

        public class ImageConfiguration : IEntityTypeConfiguration<Image>
        {
            public void Configure(EntityTypeBuilder<Image> builder)
            {
                builder.Property(p => p.AltName).HasMaxLength(200);
                builder.Property(p => p.Extension).HasMaxLength(20);
                builder.Property(p => p.Link).HasMaxLength(500);
                builder.Property(p => p.Description).HasMaxLength(2000);
            }
        }

        public class AlbumConfiguration : IEntityTypeConfiguration<Album>
        {
            public void Configure(EntityTypeBuilder<Album> builder)
            {
                builder.Property(p => p.Name).HasMaxLength(500);
                builder.Property(p => p.Description).HasMaxLength(2000);
            }
        }

        public class IdolConfiguration : IEntityTypeConfiguration<Idol>
        {
            public void Configure(EntityTypeBuilder<Idol> builder)
            {
                builder.Property(p => p.FirstName).HasMaxLength(200);
                builder.Property(p => p.LastName).HasMaxLength(200);
                builder.Property(p => p.MiddleName).HasMaxLength(200);
                builder.Property(p => p.NickName).HasMaxLength(200);
                builder.Property(p => p.Sex).HasDefaultValue(false);
                builder.Property(p => p.Star).HasMaxLength(200);
            }
        }

        public class BloodConfiguration : IEntityTypeConfiguration<Blood>
        {
            public void Configure(EntityTypeBuilder<Blood> builder)
            {
                builder.Property(p => p.Name).HasMaxLength(50);
            }
        }

        public class MappingMovieCategoryConfiguration : IEntityTypeConfiguration<MappingMovieCategory>
        {
            public void Configure(EntityTypeBuilder<MappingMovieCategory> builder)
            {
                builder.HasKey(c => new { c.MovieId, c.CategoryId });
                builder.HasOne<Movie>(a => a.Movie)
                        .WithMany(i => i.MovieCategories)
                        .HasForeignKey(sc => sc.MovieId);
                builder.HasOne<Category>(a => a.Category)
                        .WithMany(i => i.MovieCategories)
                        .HasForeignKey(sc => sc.CategoryId);

            }
        }

        public class MappingMovieIdolConfiguration : IEntityTypeConfiguration<MappingMovieIdol>
        {
            public void Configure(EntityTypeBuilder<MappingMovieIdol> builder)
            {
                builder.HasKey(c => new { c.MovieId, c.IdolId });
                builder.HasOne<Movie>(a => a.Movie)
                        .WithMany(i => i.MovieIdols)
                        .HasForeignKey(sc => sc.MovieId);
                builder.HasOne<Idol>(a => a.Idol)
                        .WithMany(i => i.MovieIdols)
                        .HasForeignKey(sc => sc.IdolId);

            }
        }

        public class MappingMovieImageConfiguration : IEntityTypeConfiguration<MappingMovieImage>
        {
            public void Configure(EntityTypeBuilder<MappingMovieImage> builder)
            {
                builder.HasKey(c => new { c.MovieId, c.ImageId });
                builder.HasOne<Movie>(a => a.Movie)
                        .WithMany(i => i.MovieImages)
                        .HasForeignKey(sc => sc.MovieId);
                builder.HasOne<Image>(a => a.Image)
                        .WithMany(i => i.MovieImages)
                        .HasForeignKey(sc => sc.ImageId);

            }
        }

        public class MovieConfiguration : IEntityTypeConfiguration<Movie>
        {
            public void Configure(EntityTypeBuilder<Movie> builder)
            {
                builder.Property(p => p.Title).HasMaxLength(200);
                builder.Property(p => p.Code).HasMaxLength(50);
                builder.Property(p => p.Description).HasMaxLength(2000);
            }
        }

        public class SourceLinkConfiguration : IEntityTypeConfiguration<SourceLink>
        {
            public void Configure(EntityTypeBuilder<SourceLink> builder)
            {
                builder.Property(p => p.Name).HasMaxLength(200);
                builder.Property(p => p.Link).HasMaxLength(700);
                builder.Property(p => p.DownloadLink).HasMaxLength(700);
                builder.Property(p => p.Resolution).HasMaxLength(50);
                builder.Property(p => p.Server).HasMaxLength(200);
            }
        }

        public class MappingUserCommentConfiguration : IEntityTypeConfiguration<MappingUserComment>
        {
            public void Configure(EntityTypeBuilder<MappingUserComment> builder)
            {
                builder.HasKey(c => new { c.UserProfileId, c.CommentId });
                builder.HasOne<UserProfile>(a => a.UserProfile)
                        .WithMany(i => i.UserComments)
                        .HasForeignKey(sc => sc.UserProfileId);
                builder.HasOne<Comment>(a => a.Comment)
                        .WithMany(i => i.UserComments)
                        .HasForeignKey(sc => sc.CommentId);

            }
        }

        public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
        {
            public void Configure(EntityTypeBuilder<UserProfile> builder)
            {
                builder.Property(p => p.FirstName).HasMaxLength(200);
                builder.Property(p => p.LastName).HasMaxLength(200);
                builder.Property(p => p.MiddleName).HasMaxLength(200);
                builder.Property(p => p.Email).HasMaxLength(200);
            }
        }
    }
}
