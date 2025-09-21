using FamilyAI.Domain.Data;
using FamilyAI.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FamilyAI.Infrastructure.Services
{
    public class ChatLogService
    {
        private readonly MyDbContext _myDbContext;

        public ChatLogService(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }

        /// <summary>
        /// Adds a new chat log entry
        /// </summary>
        /// <param name="chatLog">The chat log to add</param>
        /// <returns>The added chat log with generated ID</returns>
        public async Task<ChatLog> AddChatLogAsync(ChatLog chatLog)
        {
            if (chatLog.ThreadId.Equals(0))
            {
                _myDbContext.Threads.Add(new ThreadModel
                {
                    UserId = chatLog.UserId,
                    ThreadName = chatLog.Text.Substring(0, 10)
                });

                await _myDbContext.SaveChangesAsync();

                chatLog.ThreadId = _myDbContext.Threads.OrderByDescending(t => t.Id).First().Id;
            }


            try
            {
                // Set default values if not provided
                if (chatLog.EntryTime == default)
                {
                    chatLog.EntryTime = DateTime.UtcNow;
                }

                chatLog.IsDeleted = false;

                _myDbContext.ChatLogs.Add(chatLog);
                await _myDbContext.SaveChangesAsync();

                return chatLog;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding chat log: {ex.Message}");
                throw; // Re-throw to let caller handle
            }
        }

        /// <summary>
        /// Adds a new chat log entry with basic parameters
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="threadId">The thread ID</param>
        /// <param name="text">The message text</param>
        /// <param name="isReply">Whether this is a reply (default: false)</param>
        /// <returns>The added chat log with generated ID</returns>
        public async Task<ChatLog> AddChatLogAsync(int userId, int threadId, string text, bool isReply = false)
        {

            var chatLog = new ChatLog
            {
                UserId = userId,
                ThreadId = threadId,
                Text = text,
                IsReply = isReply,
                EntryTime = DateTime.UtcNow,
                IsDeleted = false
            };

            return await AddChatLogAsync(chatLog);
        }

        /// <summary>
        /// Soft deletes a chat log by setting IsDeleted to true
        /// </summary>
        /// <param name="chatLogId">The ID of the chat log to delete</param>
        /// <returns>True if the chat log was found and deleted, false otherwise</returns>
        public async Task<bool> DeleteChatLogAsync(int chatLogId)
        {
            var chatLog = await _myDbContext.ChatLogs
                .FirstOrDefaultAsync(cl => cl.Id == chatLogId && !cl.IsDeleted);

            if (chatLog == null)
            {
                return false;
            }

            chatLog.IsDeleted = true;
            await _myDbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Permanently deletes a chat log from the database
        /// </summary>
        /// <param name="chatLogId">The ID of the chat log to permanently delete</param>
        /// <returns>True if the chat log was found and deleted, false otherwise</returns>
        public async Task<bool> PermanentlyDeleteChatLogAsync(int chatLogId)
        {
            var chatLog = await _myDbContext.ChatLogs
                .FirstOrDefaultAsync(cl => cl.Id == chatLogId);

            if (chatLog == null)
            {
                return false;
            }

            _myDbContext.ChatLogs.Remove(chatLog);
            await _myDbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Gets all chat logs for a specific thread
        /// </summary>
        /// <param name="threadId">The thread ID</param>
        /// <returns>List of chat logs for the thread</returns>
        public async Task<List<ChatLog>> GetChatLogsByThreadAsync(int threadId)
        {
            try
            {
                var query = _myDbContext.ChatLogs
                    .AsNoTracking()
                    .Include(th => th.Thread)
                    .Include(th => th.User)
                    .Where(cl => cl.ThreadId == threadId && !cl.IsDeleted)
                    .OrderBy(cl => cl.EntryTime);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting chat logs for thread {threadId}: {ex.Message}");
                return new List<ChatLog>();
            }
        }

        /// <summary>
        /// Gets all chat logs for a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of chat logs for the user</returns>
        public async Task<List<ChatLog>> GetChatLogsByUserAsync(int userId)
        {
            try
            {
                var query = _myDbContext.ChatLogs
                    .AsNoTracking()
                    .Where(cl => cl.UserId == userId && !cl.IsDeleted)
                    .OrderByDescending(cl => cl.EntryTime);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting chat logs for user {userId}: {ex.Message}");
                return new List<ChatLog>();
            }
        }

        /// <summary>
        /// Gets a specific chat log by ID
        /// </summary>
        /// <param name="chatLogId">The chat log ID</param>
        /// <returns>The chat log if found, null otherwise</returns>
        public async Task<ChatLog?> GetChatLogByIdAsync(int chatLogId)
        {
            try
            {
                return await _myDbContext.ChatLogs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cl => cl.Id == chatLogId && !cl.IsDeleted);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting chat log {chatLogId}: {ex.Message}");
                return null;
            }
        }
    }
}