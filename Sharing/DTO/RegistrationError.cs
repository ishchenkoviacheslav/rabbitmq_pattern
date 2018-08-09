using System;
using System.Collections.Generic;
using System.Text;

namespace Sharing.DTO
{
    [Serializable]
    public class RegistrationError:BackError
    {
        public RegistrationError(string errorDescription):base(errorDescription)
        {

        }
    }
}
