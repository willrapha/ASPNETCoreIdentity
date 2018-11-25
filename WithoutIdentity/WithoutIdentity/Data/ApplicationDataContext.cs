using Microsoft.EntityFrameworkCore;

namespace WithoutIdentity.Data
{
    public class ApplicationDataContext : DbContext
    {
        public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) 
            : base(options)
        {
            
        }
    }
}