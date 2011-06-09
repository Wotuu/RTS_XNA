using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.ChatServer;
using GameServer.ChatServer.Channels;
using SocketLibrary.Users;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;

namespace GameServer.Users
{
    public class ServerUser : User
    {
        public ChatClientListener listener { get; set; }

        public ServerUser(String username, ChatClientListener listener ) : base(username)
        {
            this.id = ServerUserManager.GetInstance().RequestUserID();
            this.listener = listener;
            this.listener.client.onDisconnectListeners += this.OnDisconnect;

            ServerUserManager.GetInstance().users.AddLast(this);
        }

        
        /// <summary>
        /// When the user disconnects from the server.
        /// </summary>
        public void OnDisconnect()
        {
            ChannelManager.GetInstance().GetChannelByID(this.channelID).UserLeft(this);
            ServerUserManager.GetInstance().users.Remove(this);
        }

        /// <summary>
        /// Changes the channel of this user.
        /// </summary>
        /// <param name="newChannel"></param>
        public void ChangeChannel(int newChannel)
        {
            Console.Out.WriteLine("Leaving channel " + this.channelID);
            ChannelManager.GetInstance().GetChannelByID(this.channelID).UserLeft(this);
            Console.Out.WriteLine("Joining channel channel " + newChannel);
            ChannelManager.GetInstance().GetChannelByID(newChannel).AddUser(this);
        }
    }
}
