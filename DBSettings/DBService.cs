using Microsoft.EntityFrameworkCore;

namespace DBSettings
{
    public sealed class ApplicationContext : DbContext 
    {
        public DbSet<ProjTreeModel> ProjTree{ get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) //параметры в файл
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=sibintech;User Id=SA;Password=Administrator1;");
        }
    }
}