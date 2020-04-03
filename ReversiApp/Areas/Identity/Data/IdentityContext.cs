using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReversiApp.Areas.Identity.Data;

namespace ReversiApp.Models
{
    public class IdentityContext : IdentityDbContext<Speler>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
        }

        public DbSet<Speler> Spelers { get; set; }

        public IdentityContext() { }
    }
}
