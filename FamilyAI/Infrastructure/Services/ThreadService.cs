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
            return myDbContext.Threads
                .AsNoTracking()
                .Where(tt => tt.UserId == userid && !tt.IsDeleted)
                .OrderByDescending(tt => tt.Id)
                .ToList();
        }

        public async Task<bool> DeleteThreadAsync(int threadId)
        {
            var thread = await myDbContext.Threads
                .FirstOrDefaultAsync(t => t.Id == threadId && !t.IsDeleted);

            if (thread == null) return false;

            // Soft-delete all messages in the thread too
            var logs = await myDbContext.ChatLogs
                .Where(cl => cl.ThreadId == threadId && !cl.IsDeleted)
                .ToListAsync();

            foreach (var log in logs)
                log.IsDeleted = true;

            thread.IsDeleted = true;
            await myDbContext.SaveChangesAsync();
            return true;
        }



    }
}
