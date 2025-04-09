using Microsoft.EntityFrameworkCore;
using Vendsys.Models;

namespace Vendsys.Data
{
    public class Connection : DbContext
    {
        public Connection(DbContextOptions<Connection> options)
         : base(options)
        {
        }

        public DbSet<DexMeter> DexMeter { get; set; }
        public DbSet<DexLaneMeter> DexLaneMeter { get; set; }

    }
}
    