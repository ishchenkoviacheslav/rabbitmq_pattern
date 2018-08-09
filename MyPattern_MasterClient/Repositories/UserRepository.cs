using MyLogger;
using MyPattern_MasterClient.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MyPattern_MasterClient.Repositories
{
    static class UserRepository
    {
        public static void AddUser(User user)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        public static void DeleteUser(User user)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Remove(user);
                db.SaveChanges();
            }
        }
        //public void DeleteUserByName(string userName)
        //{
        //    using (ApplicationContext db = new ApplicationContext())
        //    {
        //        db.Remove(user);
        //        db.SaveChanges();
        //    }
        //}

        //public void DeleteUserById(int id)
        //{
        //    using (ApplicationContext db = new ApplicationContext())
        //    {
        //        db.Remove(user);
        //        db.SaveChanges();
        //    }
        //}
        //what can be changed in "User"?
        public static void SetUserSession(int userId, Guid session)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                User findedUser = db.Users.FirstOrDefault((u) => u.Id == userId);
                findedUser.SessionId = session;
                db.SaveChangesAsync();
            }
        }
        //can be null !!!
        public static User GetUserById(int userId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return db.Users.FirstOrDefault((u) => u.Id == userId);
            }
        }

        public static User GetUserByEmail(string email)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return db.Users.FirstOrDefault((u) => u.Email == email);
            }
        }

        public static bool IsExistEmail(string email)
        {
            bool isExist = false;
            using (ApplicationContext db = new ApplicationContext())
            {
                isExist = db.Users.FirstOrDefault((u) => u.Email == email) == null ? false : true;
            }
            return isExist;
        }

        private static void AllExceptions(Exception ex)
        {
            if (ex.InnerException != null)
            {
                AllExceptions(ex.InnerException);
                return;
            }
            Logger.Error(ex.Message);
        }

        public static string HashCode(string myDataEncoded)
        {
            SHA1 sha = SHA1.Create();
            byte[] bytes = new ASCIIEncoding().GetBytes(myDataEncoded);
            sha.ComputeHash(bytes);
            myDataEncoded = Convert.ToBase64String(sha.Hash);
            return myDataEncoded;
        }
    }
}
