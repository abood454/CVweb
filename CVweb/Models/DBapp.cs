using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CVweb.Models
{
    public class DBapp :IdentityDbContext
    {
        public DBapp(DbContextOptions<DBapp> op) : base(op)
        {

        }
        public DbSet<CVz> CVzs { get; set; }
    }
}
