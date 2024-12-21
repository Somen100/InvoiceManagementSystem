using InvoiceMgmt.DAL.Data;
using InvoiceMgmt.DAL.IRepo;
using InvoiceMgmt.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceMgmt.DAL.Repo
{
    public class RoleMasterRepo : IRoleMasterRepo
    {
        private readonly InvoiceDbContext _context;
        public RoleMasterRepo(InvoiceDbContext context)
        {
            _context = context;
        }

        public async Task<RoleMaster> CreateRoleAsync(RoleMaster role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<RoleMaster> GetRoleByIdAsync(int id)
        {
            return await _context.Roles
                                 .Include(r => r.Users)  // To include users associated with the role
                                 .FirstOrDefaultAsync(r => r.RoleId == id);
        }

        public async Task<IEnumerable<RoleMaster>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<RoleMaster> UpdateRoleAsync(RoleMaster role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(p => p.RoleId == id);
            if (role == null)
            {
                throw new Exception("Role Not Found");
            }
            if (role != null)
            {
                role.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}
