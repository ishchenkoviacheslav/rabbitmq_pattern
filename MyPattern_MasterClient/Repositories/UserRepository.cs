using MyLogger;
using MyPattern_MasterClient.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MyPattern_MasterClient.Repositories
{
    class UserRepository
    {
        public void AddUser(User user)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        public void DeleteUser(User user)
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
        public void EditUser(User user)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                User findedUser = db.Users.FirstOrDefault((u) => u.Id == user.Id);

                db.SaveChanges();
            }
        }
        //can be null !!!
        public User GetUserById(int userId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return db.Users.FirstOrDefault((u) => u.Id == userId);
            }
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
