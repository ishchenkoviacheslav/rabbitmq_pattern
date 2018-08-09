using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyPattern_MasterClient.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
        public Guid SessionId { get; set; }
        public string QueueName { get; set; }
    }
}
