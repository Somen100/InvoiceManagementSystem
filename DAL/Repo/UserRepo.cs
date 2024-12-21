using InvoiceMgmt.DAL.Data;
using InvoiceMgmt.DAL.IRepo;
using InvoiceMgmt.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace InvoiceMgmt.DAL.Repo
{
    public class UserRepo : IUserRepo
    {
        private readonly InvoiceDbContext _context;
        public UserRepo(InvoiceDbContext context)
        {
            _context = context;
        }

        public async Task<User> AddUser(User user)
        {
            user.PasswordHash = HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteUser(int id)
        {
            var record = await _context.FindAsync<User>(id);
            if (record == null)
            {
                throw new Exception("User Not Found");
            }
            record.IsActive = false;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync(); 
        }

        public async Task<User> GetUserByEmailOrUsername(string emailOrUsername)
        {
            var record = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailOrUsername ||  u.Username == emailOrUsername);

            if (record == null)
            {
                throw new Exception("User Not Found");
            }
            
            return record;
        }

        public async Task<User> GetUserById(int id)
        {
            var record = await _context.Users.FirstOrDefaultAsync(u=>u.UserId==id);
            if (record == null)
            {
                throw new Exception("User Not Found");
            }

            return record;
        }

        public async Task UpdateUser(int id, User user)
        {
            var record = await _context.FindAsync<User>(id);
            if (record == null)
            {
                throw new Exception("User Not Found");
            }
            record.Username = user.Username;
            record.Role = user.Role;
            record.IsActive = true;
            await _context.SaveChangesAsync();
        }

        //login:authentication
        public User Authenticate(string emailOrUsername, string password)
        {
            var user = _context.Users
                 .Include(u => u.Role)
                .SingleOrDefault(u => u.Username == emailOrUsername || u.Email == emailOrUsername);
            if (user == null || user.PasswordHash != HashPassword(password))
                return null;

            return user;
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
