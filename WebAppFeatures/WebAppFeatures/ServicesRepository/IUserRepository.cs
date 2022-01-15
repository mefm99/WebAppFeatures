using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppFeatures.Models;

namespace WebAppFeatures.ServicesRepository
{
    public interface IUserRepository
    {
        List<User> AllUser();
        string RegisterUser(User user, string salt);
        User GetUserbyUserName(string userName);
        User GetUserbyEmail(string email);
        User GetUserbyUserId(string userId);
        UserTokens GetUserSaltbyUserId(string userId);
        bool CheckUserNameExists(string userName);
        bool CheckEmailExists(string emailId);
        bool CheckUserNameEmailExists(string Usernameemail);
        bool UpdatePasswordandHistory(string userId, string passwordHash, string passwordSalt);
        bool UpdateImageUser(string userId, byte[] image);
        bool UpdateNameUser(string userId, string Name);
        bool UpdateUserNameUser(string userId, string userName);
        bool UpdateCountryUser(string userId, string Country);
        bool UpdateCityUser(string userId, string City);
        bool UpdateGenderUser(string userId, string Gender);
        bool DeleteMyProfile(string userId);
    }
}
