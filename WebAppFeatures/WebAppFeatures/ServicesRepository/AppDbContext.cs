using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppFeatures.Models;

namespace WebAppFeatures.ServicesRepository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        public DbSet<UserTokens> UserTokens { get; set; }
        public DbSet<RegisterVerification> RegisterVerification { get; set; }
        public DbSet<ResetPasswordVerification> ResetPasswordVerification { get; set; }
    }
}
