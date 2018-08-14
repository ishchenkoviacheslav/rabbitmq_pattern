using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sharing.DTO
{
    [Serializable]
    public class LogoutRequest
    {
        public LogoutRequest(string sessionid)
        {
            SessionId = sessionid;
        }
        [Required]
        public string SessionId { get; set; }
    }
}
