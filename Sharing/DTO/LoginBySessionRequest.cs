using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sharing.DTO
{
    [Serializable]
    public class LoginBySessionRequest
    {
        public LoginBySessionRequest(string session)
        {
            Session = session;
        }
        [Required]
        public string Session { get; set; }
    }
}
