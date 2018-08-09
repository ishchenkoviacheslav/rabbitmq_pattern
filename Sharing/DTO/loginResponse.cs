using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sharing.DTO
{
    [Serializable]
    public class LoginResponse
    {
        [Required]
        public string SessionId { get; set; }
    }
}
