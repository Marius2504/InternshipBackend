using DatingApp.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Data
{
    public class DataContext:DbContext  
    {
        public DataContext(DbContextOptions options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<LikeUser>()
                .HasKey(k => new { k.LikedUserId,k.SourceUserId});
            builder.Entity<LikeUser>()
                .HasOne(s => s.SourceUser)
                .WithMany(d => d.LikedUsers)
                .HasForeignKey(e => e.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<LikeUser>()
                .HasOne(s => s.LikedUser)
                .WithMany(d => d.LikedByUsers)
                .HasForeignKey(e => e.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(x => x.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Message>()
              .HasOne(u => u.Sender)
              .WithMany(x => x.MessagesSent)
              .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<LikeUser> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
