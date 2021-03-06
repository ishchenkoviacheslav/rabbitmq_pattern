﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sharing.DTO
{
    [Serializable]
    public class RegistrationRequest
    {
        public RegistrationRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
