using FamilyAI.Domain.Data;
using FamilyAI.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FamilyAI.Infrastructure.Services
{
    public class UserManagementService
    {
        private readonly MyDbContext _db;
        private readonly IPasswordHasher<UserModel> _passwordHasher;

        public UserManagementService(MyDbContext db, IPasswordHasher<UserModel> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _db.Users
                .AsNoTracking()
                .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Role)
                .Where(u => !u.IsDeleted)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Username = u.Username,
                    RoleId = u.UserPermissions.RoleId,
                    RoleName = u.UserPermissions.Role.RoleName
                })
                .ToListAsync();
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _db.Roles
                .AsNoTracking()
                .Where(r => !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<Role> CreateRoleAsync(string roleName)
        {
            var role = new Role { RoleName = roleName, IsDeleted = false };
            _db.Roles.Add(role);
            await _db.SaveChangesAsync();
            return role;
        }

        public async Task<bool> UpdateRoleAsync(int id, string roleName)
        {
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
            if (role == null) return false;
            role.RoleName = roleName;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            // Prevent deletion if any users are assigned this role
            bool inUse = await _db.UserPermissions.AnyAsync(up => up.RoleId == id && !up.IsDeleted);
            if (inUse) return false;

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
            if (role == null) return false;
            role.IsDeleted = true;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserDto>> GetAllParentsAsync()
        {
            return await _db.Users
                .AsNoTracking()
                .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Role)
                .Where(u => !u.IsDeleted && u.UserPermissions.Role.RoleName == "Parent")
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Username = u.Username,
                    RoleId = u.UserPermissions.RoleId,
                    RoleName = u.UserPermissions.Role.RoleName
                })
                .ToListAsync();
        }

        public async Task<List<int>> GetLinkedParentIdsAsync(int childId)
        {
            return await _db.ParentChildren
                .AsNoTracking()
                .Where(pc => !pc.IsDeleted && pc.ChildId == childId)
                .Select(pc => pc.ParentId)
                .ToListAsync();
        }

        public async Task<int> GetUserCountForRoleAsync(int roleId)
        {
            return await _db.UserPermissions.CountAsync(up => up.RoleId == roleId && !up.IsDeleted);
        }

        public async Task<List<UserDto>> GetAllChildrenAsync()
        {
            return await _db.Users
                .AsNoTracking()
                .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Role)
                .Where(u => !u.IsDeleted && u.UserPermissions.Role.RoleName == "Child")
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Username = u.Username,
                    RoleId = u.UserPermissions.RoleId,
                    RoleName = u.UserPermissions.Role.RoleName
                })
                .ToListAsync();
        }

        public async Task<List<int>> GetLinkedChildIdsAsync(int parentId)
        {
            return await _db.ParentChildren
                .AsNoTracking()
                .Where(pc => !pc.IsDeleted && pc.ParentId == parentId)
                .Select(pc => pc.ChildId)
                .ToListAsync();
        }

        public async Task<List<UserDto>> GetChildrenForParentAsync(int parentId)
        {
            return await _db.ParentChildren
                .AsNoTracking()
                .Include(pc => pc.Child)
                .ThenInclude(c => c.UserPermissions)
                .ThenInclude(up => up.Role)
                .Where(pc => !pc.IsDeleted && pc.ParentId == parentId)
                .Select(pc => new UserDto
                {
                    Id = pc.Child.Id,
                    Name = pc.Child.Name,
                    Username = pc.Child.Username,
                    RoleId = pc.Child.UserPermissions.RoleId,
                    RoleName = pc.Child.UserPermissions.Role.RoleName
                })
                .ToListAsync();
        }

        public async Task<UserModel> CreateUserAsync(string name, string username, string password, int roleId)
        {
            var user = new UserModel
            {
                Name = name,
                Username = username,
                IsDeleted = false
            };
            user.Password = _passwordHasher.HashPassword(user, password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            _db.UserPermissions.Add(new UserPermission
            {
                UserId = user.Id,
                RoleId = roleId,
                IsDeleted = false
            });
            await _db.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UpdateUserAsync(int id, string name, string username, int roleId)
        {
            var user = await _db.Users
                .Include(u => u.UserPermissions)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

            if (user == null) return false;

            user.Name = name;
            user.Username = username;

            if (user.UserPermissions != null)
            {
                user.UserPermissions.RoleId = roleId;
            }
            else
            {
                _db.UserPermissions.Add(new UserPermission
                {
                    UserId = id, RoleId = roleId, IsDeleted = false
                });
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResetPasswordAsync(int id, string newPassword)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            if (user == null) return false;

            user.Password = _passwordHasher.HashPassword(user, newPassword);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            if (user == null) return false;

            user.IsDeleted = true;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LinkParentToChildAsync(int parentId, int childId)
        {
            bool exists = await _db.ParentChildren
                .AnyAsync(pc => pc.ParentId == parentId && pc.ChildId == childId && !pc.IsDeleted);

            if (exists) return false;

            _db.ParentChildren.Add(new ParentChild
            {
                ParentId = parentId,
                ChildId = childId,
                IsDeleted = false
            });
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnlinkParentFromChildAsync(int parentId, int childId)
        {
            var link = await _db.ParentChildren
                .FirstOrDefaultAsync(pc => pc.ParentId == parentId && pc.ChildId == childId && !pc.IsDeleted);

            if (link == null) return false;

            link.IsDeleted = true;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
