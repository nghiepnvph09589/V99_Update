using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Data.Model.APIApp
{
    public class LoginAppInputModel
    {
        public string Phone { get; set; }
        public string Password { get; set; }
        public string DeviceID { get; set; }
        public string Name { get; set; }
        public string LastRefCode { get; set; }
        public string Email { get; set; }
    }
}