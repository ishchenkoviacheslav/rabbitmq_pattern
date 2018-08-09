using System;
using System.Collections.Generic;
using System.Text;

namespace Sharing.DTO
{
    [Serializable]
    public class BackError
    {
        public BackError(string error)
        {
            ErrorDescription = error;
        }
        public string ErrorDescription { get; set; }
    }
}
