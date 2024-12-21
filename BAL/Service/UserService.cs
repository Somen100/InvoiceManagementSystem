using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.DAL.IRepo;
using InvoiceMgmt.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceMgmt.BAL.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<User> AddUser(User user)
        {
            return await _userRepo.AddUser(user);
        }

        public Task DeleteUser(int id)
        {
           return  _userRepo.DeleteUser(id);
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
           return await _userRepo.GetAllUsers();
        }

        public async Task<User> GetUserByEmailOrUsername(string emailOrUsername)
        {
           return await _userRepo.GetUserByEmailOrUsername(emailOrUsername); ;
        }

        public async Task<User> GetUserById(int id)
        {
           return await _userRepo.GetUserById(id);
        }

        public Task UpdateUser(int id, User user)
        {
            return _userRepo.UpdateUser(id, user);
        }

        public async Task<User> ValidateUserCredentials(string emailOrUsername, string password)
        {
            var user = await _userRepo.GetUserByEmailOrUsername(emailOrUsername);
            if (user == null)
            {
                return null;
            }

            // Validate the password (you should hash the password in real applications)
            if (user.PasswordHash != password)
            {
                return null;
            }

            return user;
        }

        public User Authenticate(string emailOrUsername, string password)
        {
            return _userRepo.Authenticate(emailOrUsername, password);
        }

    }
}
