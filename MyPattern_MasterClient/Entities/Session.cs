using System;
using System.Collections.Generic;
using System.Text;

namespace MyPattern_MasterClient.Entities
{
    public class Session
    {
        public string SessionId { get; set; }
        public int UserId { get; set; }//foreign key for this table to Users table - relationship one to one
        public User User { get; set; } // just navigation properties

    }
}
