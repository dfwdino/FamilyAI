using FamilyAI.Domain.Data;
using FamilyAI.Domain.Models;
using System.Reflection.Metadata.Ecma335;

namespace FamilyAI.Infrastructure.Services
{
    public class UserServcies
    {

        private readonly MyDbContext myDbContext;

        public UserServcies(MyDbContext myDbContext)
        {
            this.myDbContext = myDbContext;
        }

        public Task<int?> SignIn(UserModel user)
        {
            int? userid = myDbContext.Users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password)?.Id;

            return Task.FromResult(userid);
        }
    }
}
