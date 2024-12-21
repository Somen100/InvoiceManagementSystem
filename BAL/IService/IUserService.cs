using InvoiceMgmt.Models;

namespace InvoiceMgmt.BAL.IService
{
    public interface IUserService
    {
        Task<User> AddUser(User user);
        Task UpdateUser(int id, User user);
        Task DeleteUser(int id);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> GetUserById(int id);
        Task<User> GetUserByEmailOrUsername(string emailOrUsername);

        //login:authentication
        User Authenticate(string username, string password);
    }
}
