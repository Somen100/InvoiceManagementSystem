using InvoiceMgmt.Models;

namespace InvoiceMgmt.DAL.IRepo
{
    public interface IUserRepo
    {
        Task<User> AddUser(User user);
        Task UpdateUser(int id, User user);
        Task DeleteUser(int id);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> GetUserById(int id);

        //login:authentication
        Task<User> GetUserByEmailOrUsername(string emailOrUsername);
        User Authenticate(string username, string password);

    }
}
