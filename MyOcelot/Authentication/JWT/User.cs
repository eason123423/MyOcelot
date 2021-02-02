using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyOcelot.Authentication.JWT
{
    /// <summary>
    /// 登录用户信息
    /// </summary>
    public class User
    {
        public int UserID { get; set; }
        public string Email { get; set; }

        public string Name { get; set; }
        public string Role { get; set; }


        public User(int userID, string name, string email, string role)
        {
            this.UserID = userID;
            this.Name = name;
            this.Email = email;
            this.Role = role;
        }
    }
}
