using System.Data.Entity;
using CIAuth.DataAccess.Model;

namespace CIAuth.DataAccess
{
    public class CIAuthContext:DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Grant> Grants { get; set; }

    }
}