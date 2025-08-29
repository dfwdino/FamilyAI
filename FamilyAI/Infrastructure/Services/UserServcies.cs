using FamilyAI.Domain.Data;
using FamilyAI.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FamilyAI.Infrastructure.Services
{
    public class UserServcies
    {

        private readonly MyDbContext myDbContext;

        public UserServcies(MyDbContext myDbContext)
        {
            this.myDbContext = myDbContext;
        }

        public Task<AuthenticatedUser?> SignIn(LoginModel logininfo)
        {
            UserModel? user = myDbContext.Users
                .AsNoTracking()
                .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Role)
                .Where(u => u.IsDeleted.Equals(false))
                .FirstOrDefault(u => u.Username == logininfo.Username && u.Password == logininfo.Password);

            if (user == null)
            {
                return Task.FromResult<AuthenticatedUser?>(null);
            }

            AuthenticatedUser authUser = new();

            authUser.UserId = user.Id;

            authUser.RoleName = user.UserPermissions.Role.RoleName;

            return Task.FromResult<AuthenticatedUser?>(authUser);
        }
    }
}
