using FamilyAI.Domain.Data;
using FamilyAI.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FamilyAI.Infrastructure.Services
{
    public class UserServices
    {
        private readonly MyDbContext _myDbContext;
        private readonly IPasswordHasher<UserModel> _passwordHasher;

        public UserServices(MyDbContext myDbContext, IPasswordHasher<UserModel> passwordHasher)
        {
            _myDbContext = myDbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthenticatedUser?> GetAuthenticatedUserByIdAsync(int userId)
        {
            var user = await _myDbContext.Users
                .AsNoTracking()
                .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Role)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user?.UserPermissions?.Role == null) return null;

            return new AuthenticatedUser
            {
                UserId   = user.Id,
                Name     = user.Name,
                RoleId   = user.UserPermissions.RoleId,
                RoleName = user.UserPermissions.Role.RoleName
            };
        }

        public Task<AuthenticatedUser?> SignIn(LoginModel logininfo)
        {
            UserModel? user = _myDbContext.Users
                .AsNoTracking()
                .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Role)
                .Where(u => !u.IsDeleted)
                .FirstOrDefault(u => u.Username == logininfo.Username);

            if (user == null)
                return Task.FromResult<AuthenticatedUser?>(null);

            // Support both hashed and legacy plaintext passwords during migration
            bool passwordValid = false;
            try
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.Password, logininfo.Password);
                passwordValid = result != PasswordVerificationResult.Failed;
            }
            catch
            {
                // Stored value was not a valid hash — fall through to plaintext check
            }

            if (!passwordValid)
                passwordValid = user.Password == logininfo.Password;

            if (!passwordValid)
                return Task.FromResult<AuthenticatedUser?>(null);

            if (user.UserPermissions?.Role == null)
                return Task.FromResult<AuthenticatedUser?>(null); // user has no role assigned

            var authUser = new AuthenticatedUser
            {
                UserId   = user.Id,
                Name     = user.Name,
                RoleId   = user.UserPermissions.RoleId,
                RoleName = user.UserPermissions.Role.RoleName
            };

            return Task.FromResult<AuthenticatedUser?>(authUser);
        }
    }
}
