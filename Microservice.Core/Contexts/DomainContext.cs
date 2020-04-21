namespace $safeprojectname$.Contexts
{
    using System;
    using Microsoft.EntityFrameworkCore;

    [Obsolete("This class is just an example template")]
    public class DomainContext : DbContext
    {
        public DomainContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
