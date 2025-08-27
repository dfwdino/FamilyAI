using Microsoft.EntityFrameworkCore;

namespace FamilyAI.Domain.Data
{
    public class MyDbContext : DbContext
    {

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }
    }
}
