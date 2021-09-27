using BlazorGuessTheElo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.DataContext
{
    public class EloSubmissionDatabaseContext : DbContext
    {

        public DbSet<EloSubmission> EloSubmissions { get; set; }
        public DbSet<AllowedChannel> AllowedChannels { get; set; }

        public EloSubmissionDatabaseContext(DbContextOptions<EloSubmissionDatabaseContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AllowedChannel>(entity =>
            {
                entity.HasKey(x => x.ChannelId);
            });
        }
    }
}
