using FamilyAI.Application.Interfaces;
using FamilyAI.Domain.Data;
using FamilyAI.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FamilyAI.Infrastructure.Services
{
    public class ThreadService
    {
        private readonly IRepository<ThreadModel> _repo;
        private readonly MyDbContext myDbContext;

        public ThreadService(MyDbContext myDbContext)
        {
            //_repo = repo;
            this.myDbContext = myDbContext;
        }

        public List<ThreadModel> GetAllThreadsByUser(int userid)
        {
            var threads = myDbContext.Threads
                            .AsNoTracking()
                            .Where(tt => tt.UserId.Equals(userid));

            return threads.ToList();
        }



    }
}
