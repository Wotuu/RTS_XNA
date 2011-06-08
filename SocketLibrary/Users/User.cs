using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketLibrary.Users
{
    public class User
    {
        public int id { get; set; }
        public String username { get; set; }
        public int channelID { get; set; }

        public User(String username)
        {
            this.username = username;
        }

        public override string ToString()
        {
            return id + " -> " + username;
        }
    }
}
