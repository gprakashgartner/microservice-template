namespace $safeprojectname$.Contexts
{
    using System;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// This context should be used when 'dirty reads' are allowed
    /// </summary>
    [Obsolete("This class is just an example template")]
    public class DomainReadUncommittedContext : DomainContext
    {
        public DomainReadUncommittedContext(DbContextOptions options)
            : base(options)
        {
            Database.ExecuteSqlRaw("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
        }
    }
}
