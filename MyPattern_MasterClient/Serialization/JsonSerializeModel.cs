using System;
using System.Collections.Generic;
using System.Text;

namespace MyPattern_MasterClient.Serialization
{
    public class JsonSerializeModel
    {
        //public string Email { get; set; }
        //public string EmailPassword { get; set; }
        //public string EmailServerIPorDomenAndPort { get; set; }

        public string DbServerDomenOrIP { get; set; }
        public string DbName { get; set; }
        public string DbUserName { get; set; }
        public string DbUserPassword { get; set; }
    }
}
