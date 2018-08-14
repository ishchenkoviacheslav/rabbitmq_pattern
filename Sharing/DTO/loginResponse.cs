using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sharing.DTO
{
    [Serializable]
    public class LoginResponse
    {
        public LoginResponse(string sessionId)
        {
            SessionId = sessionId;
        }
        [Required]
        public string SessionId { get; set; }
    }
}
