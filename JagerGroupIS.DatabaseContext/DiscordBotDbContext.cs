using JagerGroupIS.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagerGroupIS.DatabaseContext
{
    public class DiscordBotDbContext : DbContext
    {
        public virtual DbSet<Election> Elections { get; set; }

        public virtual DbSet<RoleElectionSetup> RoleElectionSetups { get; set;}

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Vote> Votes { get; set; }

        public virtual DbSet<TrackingMessage> TrackingMessages { get; set; }

        public static string ConnectionString;

        ILoggerFactory loggerFactory { get; set; }

        public DiscordBotDbContext()
        {
            loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();

            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseNpgsql(ConnectionString)
                              //.UseLoggerFactory(loggerFactory)
                              .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
        }
    }
}
