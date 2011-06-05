using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Multiplayer.Users;

namespace PathfindingTest.Multiplayer.SocketConnection
{
    public class User
    {
        public int id { get; set; }
        public String username { get; set; }
        public int channel { get; set; }

        public User(String username)
        {
            this.username = username;
            UserManager.GetInstance().users.AddLast(this);
        }
    }
}
