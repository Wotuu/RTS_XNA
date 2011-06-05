using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Users
{
    public class UserManager
    {
        public LinkedList<User> users = new LinkedList<User>();

        private static UserManager instance;

        private UserManager() { }

        public static UserManager GetInstance()
        {
            if (instance == null) instance = new UserManager();
            return instance;
        }

        /// <summary>
        /// Requests a new user ID.
        /// </summary>
        /// <returns>The ID.</returns>
        public int RequestUserID()
        {
            if (users.Count == 0) return 1;
            else return users.Last.Value.id + 1;
        }
    }
}
