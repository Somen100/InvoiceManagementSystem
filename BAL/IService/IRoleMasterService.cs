using InvoiceMgmt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceMgmt.BAL.IService
{
    public interface IRoleMasterService
    {
        Task<RoleMaster> CreateRoleAsync(RoleMaster role);
        Task<RoleMaster> GetRoleByIdAsync(int id);
        Task<IEnumerable<RoleMaster>> GetAllRolesAsync();
        Task<RoleMaster> UpdateRoleAsync(RoleMaster role);
        Task DeleteRoleAsync(int id);
    }
}
