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

        public virtual DbSet<DiscordToken> DiscordTokens { get; set; }

        public virtual DbSet<AccessTable> AccessTables { get; set; }

        public string ConnectionString { get; set; }

        ILoggerFactory loggerFactory { get; set; }

        public DiscordBotDbContext()
        {
            ConnectionString = "Host=172.19.0.2;Port=5432;Database=jagerdb;Username=postgres;Password=qwe123qwe";

            loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            base.OnConfiguring(optionsBuilder);

            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseNpgsql(ConnectionString)
                              .UseLoggerFactory(loggerFactory)
                              .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
        }
    }
}
