using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Kleimenov_AS_22_04.Models;

namespace Kleimenov_AS_22_04.Data
{
    public class Kleimenov_AS_22_04Context : DbContext
    {
        public Kleimenov_AS_22_04Context (DbContextOptions<Kleimenov_AS_22_04Context> options)
            : base(options)
        {
        }

        public DbSet<Kleimenov_AS_22_04.Models.Movie> Movie { get; set; } = default!;
    }
}
