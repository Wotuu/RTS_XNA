using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Multiplayer.SocketConnection;

namespace PathfindingTest.Multiplayer.Users
{
    public class UserManager
    {
        public LinkedList<User> users = new LinkedList<User>();

        private static UserManager instance { get; set; }

        private UserManager() { }

        public static UserManager GetInstance()
        {
            if (instance == null) instance = new UserManager();
            return instance;
        }
    }
}
