using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.DAL.IRepo;
using InvoiceMgmt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceMgmt.BAL.Service
{
    public class RoleMasterService : IRoleMasterService
    {
        private readonly IRoleMasterRepo _roleMasterRepository;

        public RoleMasterService(IRoleMasterRepo roleMasterRepo)
        {
            _roleMasterRepository = roleMasterRepo;
        }

        public async Task<RoleMaster> CreateRoleAsync(RoleMaster role)
        {
            return await _roleMasterRepository.CreateRoleAsync(role);
        }

        public async Task<RoleMaster> GetRoleByIdAsync(int id)
        {
            return await _roleMasterRepository.GetRoleByIdAsync(id);
        }

        public async Task<IEnumerable<RoleMaster>> GetAllRolesAsync()
        {
            return await _roleMasterRepository.GetAllRolesAsync();
        }

        public async Task<RoleMaster> UpdateRoleAsync(RoleMaster role)
        {
            return await _roleMasterRepository.UpdateRoleAsync(role);
        }

        public async Task DeleteRoleAsync(int id)
        {
            await _roleMasterRepository.DeleteRoleAsync(id);
        }
    }
}
