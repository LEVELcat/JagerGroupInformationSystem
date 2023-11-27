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
        //public virtual DbSet<Election> Elections { get; set; }

        //public virtual DbSet<RoleElectionSetup> RoleElectionSetups { get; set;}

        public virtual DbSet<User> Users { get; set; }

        //public virtual DbSet<Vote> Votes { get; set; }

        //public virtual DbSet<TrackingMessage> TrackingMessages { get; set; }

        public string ConnectionString { get; set; }

        ILoggerFactory loggerFactory { get; set; }

        public DiscordBotDbContext(string connectionString)
        {
            ConnectionString = connectionString;
            loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });


            Database.EnsureDeleted();

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ConnectionString = "User=SYSDBA;Password=masterkey;Database=/databases/TESTDB.FDB;DataSource=localhost;Port=3050;";

            base.OnConfiguring(optionsBuilder);

            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseFirebird(ConnectionString)
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
