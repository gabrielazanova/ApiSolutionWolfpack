using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSolutionWolfpack.Models
{
    public class DatabaseContext : DbContext
    {

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }
        public DbSet<Wolf> Wolves { get; set; }
        public DbSet<Pack> Packs { get; set; }
        public DbSet<WolfPack> WolfPacks { get; set; }

    }
}
