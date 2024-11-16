using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PinterestApp.Models;

namespace PinterestApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<SavedBookmark> SavedBookmarks { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //definirea primary key compus
            modelBuilder.Entity<SavedBookmark>()
                .HasKey(sb => new { sb.Id, sb.BookmarkId, sb.BoardId });
            //definirea relatiilor cu modelele Bookmark si Saving
            modelBuilder.Entity<SavedBookmark>()
                .HasOne(sb => sb.Bookmark)
                .WithMany(sb => sb.SavedBookmarks)
                .HasForeignKey(sb => sb.BookmarkId);
            modelBuilder.Entity<SavedBookmark>()
                .HasOne(sb => sb.Board)
                .WithMany(sb => sb.SavedBookmarks)
                .HasForeignKey(sb => sb.BoardId);
        }
    }
}