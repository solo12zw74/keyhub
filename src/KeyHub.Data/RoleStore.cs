using KeyHub.Model;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace KeyHub.Data
{
    public class RoleStore : IRoleStore<Role, int>
    {
        private readonly DataContext _context;

        public RoleStore(DataContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            _context = context;
        }

        public Task CreateAsync(Role role)
        {
            _context.Roles.Add(role);
            return _context.SaveChangesAsync();
        }

        public Task DeleteAsync(Role role)
        {
            // This method is currently unused.
            throw new NotSupportedException();
        }

        public Task<Role> FindByIdAsync(int roleId)
        {
            return _context.Roles.SingleOrDefaultAsync(r => r.Id == roleId);
        }

        public Task<Role> FindByNameAsync(string roleName)
        {
            return _context.Roles.SingleOrDefaultAsync(r => r.Name == roleName);
        }

        public Task UpdateAsync(Role role)
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
        }
    }
}
