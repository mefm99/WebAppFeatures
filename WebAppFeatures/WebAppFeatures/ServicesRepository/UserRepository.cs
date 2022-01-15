using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using WebAppFeatures.Models;

namespace WebAppFeatures.ServicesRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext context;
        public UserRepository(AppDbContext _context)
        {
            context = _context;
        }
        public List<User> AllUser()
        {
            return context.User.ToList();
        }
        public string RegisterUser(User user, string salt)
        {
            var dbContextTransaction = context.Database.BeginTransaction();
            string result = "";
            try
            {
                context.User.Add(user);
                context.SaveChanges();
                result = user.UserId;
                UserTokens userTokens = new ()
                {
                    Id = 0,
                    UserId = result,
                    PasswordSalt = salt,
                    CreatedDate = DateTime.Now
                };
                context.UserTokens.Add(userTokens);
                context.SaveChanges();
                dbContextTransaction.Commit();
                return result;
            }
            catch (Exception)
            {
                dbContextTransaction.Rollback();
                return result;
            }

        }
        public User GetUserbyUserName(string userName)
        {
            User user = context.User.Where(a => a.UserName == userName).FirstOrDefault();
            return user;
        }
        public User GetUserbyEmail(string Email)
        {
            User user = context.User.Where(a => a.Email == Email).FirstOrDefault();
            return user;
        }
        public User GetUserbyUserId(string userId)
        {
            User user = context.User.Where(a => a.UserId == userId).FirstOrDefault();
            return user;
        }
        public UserTokens GetUserSaltbyUserId(string userId)
        {
            UserTokens userToken = context.UserTokens.Where(a => a.UserId == userId).FirstOrDefault();
            return userToken;
        }
        public bool CheckUserNameExists(string userName)
        {
            bool result = context.User.Any(a => a.UserName == userName);
            return result;
        }
        public bool CheckEmailExists(string email)
        {
            bool result = context.User.Any(a => a.Email == email);
            return result;
        }
        public bool CheckUserNameEmailExists(string Usernameemail)
        {
            List<string> emails = context.User.Select(a => a.Email).ToList();
            foreach (var email in emails)
            {
                if(new MailAddress(email).User == Usernameemail)
                {
                    return true;
                }
            }
            return false;
        }
        public bool UpdatePasswordandHistory(string userId, string passwordHash, string passwordSalt)
        {
            try
            {
                var user = context.User.Where(a => a.UserId == userId).FirstOrDefault();
                var token = context.UserTokens.Where(a => a.UserId == userId).FirstOrDefault();
                token.PasswordSalt = passwordSalt;
                token.CreatedDate = DateTime.Now;
                user.PasswordHash = passwordHash;    
                context.SaveChanges();
                return true;
            }
            catch 
            {
                return false;
            }  
        }
        public bool UpdateImageUser(string userId, byte[] image)
        {
            User UserEdit = context.User.Where(a => a.UserId == userId).FirstOrDefault();
            if (UserEdit != null)
            {
                try
                {
                    UserEdit.Image = image;
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        public bool UpdateNameUser(string userId, string Name)
        {
            User UserEdit = context.User.Where(a => a.UserId == userId).FirstOrDefault();
            if (UserEdit != null)
            {
                try
                {
                    UserEdit.FullName = Name;
                    context.SaveChanges();
                    return true;

                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        public bool UpdateUserNameUser(string userId, string userName)
        {
            User UserEdit = context.User.Where(a => a.UserId == userId).FirstOrDefault();
            if (UserEdit != null)
            {
                try
                {
                    UserEdit.UserName = userName;
                    context.SaveChanges();
                    return true;

                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        public bool UpdateCountryUser(string userId, string Country)
        {
            User UserEdit = context.User.Where(a => a.UserId == userId).FirstOrDefault();
            if (UserEdit != null)
            {
                try
                {
                    UserEdit.Country = Country;
                    context.SaveChanges();
                    return true;

                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        public bool UpdateCityUser(string userId, string City)
        {
            User UserEdit = context.User.Where(a => a.UserId == userId).FirstOrDefault();
            if (UserEdit != null)
            {
                try
                {
                    UserEdit.City = City;
                    context.SaveChanges();
                    return true;

                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        public bool UpdateGenderUser(string userId, string Gender)
        {
            User UserEdit = context.User.Where(a => a.UserId == userId).FirstOrDefault();
            if (UserEdit != null)
            {
                try
                {
                    UserEdit.Gender = Gender;
                    context.SaveChanges();
                    return true;

                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        public bool DeleteMyProfile(string userId)
        {
            User UserEdit = context.User.Where(a => a.UserId == userId).FirstOrDefault();
            if (UserEdit != null)
            {
                try
                {
                    UserEdit.Deleted = true;
                    context.SaveChanges();
                    return true;

                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

    }
}
