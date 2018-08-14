using Microsoft.Extensions.Logging;
using MyLogger;
using MyPattern_MasterClient.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyPattern_MasterClient.Repositories
{
    static class SessionRepository
    {
        public static void SetUserSession(int userId, string session)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                //at first to find if userId is already exist in table
                заменить старый новым или послать ошибку
                db.Sessions.AddAsync(new Session() {UserId = userId,SessionId = session });
                db.SaveChangesAsync();
            }
        }

        public static void DeleteUserSession(string session)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Session SessionForDelete = db.Sessions.FirstOrDefault(s => s.SessionId == session);
                if(SessionForDelete==null)
                {
                    Logger.Error(nameof(DeleteUserSession) + ": session didn't found in db");
                }
                else
                {
                    db.Remove(SessionForDelete);
                    db.SaveChangesAsync();
                }
                
            }
        }

        public static User GetUserBySession(string session)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return db.Sessions.FirstOrDefault(s => s.SessionId == session)?.User;//use navigate properties
            }
        }

    }
}
