using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CIAUTH.Code
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string SessionId { get; set; }
        public bool PasswordChangeRequired { get; set; }
    }
}