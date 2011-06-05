using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.ChatServer;
using GameServer.ChatServer.Channels;

namespace GameServer.Users
{
    public class User
    {
        public int id { get; set; }
        public String username { get; set; }
        public ChatClientListener client { get; set; }

        public User(String username, ChatClientListener client )
        {
            this.id = UserManager.GetInstance().RequestUserID();
            this.username = username;
            this.client = client;
            this.client.client.onDisconnectListeners += this.onDisconnect;

            UserManager.GetInstance().users.AddLast(this);
        }

        
        /// <summary>
        /// When the user disconnects from the server.
        /// </summary>
        public void onDisconnect()
        {
            ChannelManager.GetInstance().GetChannelByID(this.id).UserLeft(this);
            UserManager.GetInstance().users.Remove(this);
        }
    }
}
