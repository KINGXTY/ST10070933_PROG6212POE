using PROG6212POE.Models.DBEntities;
using Microsoft.EntityFrameworkCore;
using PROG6212POE.Models;


namespace PROG6212POE.DAL
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions options) : base(options) 
        { 
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
