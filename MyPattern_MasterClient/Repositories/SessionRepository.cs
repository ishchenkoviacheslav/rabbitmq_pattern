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
        public static void SetUserSession(int userId, Guid session)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Sessions.AddAsync(new Session() {UserId = userId,SessionId = session });
                db.SaveChangesAsync();
            }
        }

        public static void DeleteUserSession(Guid session)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Session SessionForDelete = db.Sessions.FirstOrDefault(s => s.SessionId == session);
                if(SessionForDelete==null)
                {
                    Logger.Error(nameof(DeleteUserSession) + ":session didn't found in db, sessionForDelete is null");
                }
                db.Remove(SessionForDelete);
                db.SaveChangesAsync();
            }
        }

        public static User GetUserBySession(Guid session)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return db.Sessions.FirstOrDefault(s => s.SessionId == session)?.User;//use navigate properties
            }
        }

    }
}
