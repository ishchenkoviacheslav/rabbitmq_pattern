using System;
using System.Collections.Generic;
using System.Text;

namespace Sharing.DTO
{
    [Serializable]
    public class LoginBySessionError: BackError
    {
        public LoginBySessionError(string errorDescription): base(errorDescription)
        {

        }
    }
}
