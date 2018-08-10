using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sharing.DTO
{
    [Serializable]
    public class LogoutRequest
    {
        [Required]
        public string SessionId { get; set; }
    }
}
