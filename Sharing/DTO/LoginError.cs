using System;
using System.Collections.Generic;
using System.Text;

namespace Sharing.DTO
{
    [Serializable]
    public class LoginError: BackError
    {
        public LoginError(string errorDescription):base(errorDescription)
        {

        }
    }
}
