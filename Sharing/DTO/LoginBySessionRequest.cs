using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sharing.DTO
{
    [Serializable]
    public class LoginBySessionRequest
    {
        [Required]
        public string Session { get; set; }
    }
}
