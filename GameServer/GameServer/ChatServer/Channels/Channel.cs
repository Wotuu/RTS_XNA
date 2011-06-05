using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Users;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;

namespace GameServer.ChatServer.Channels
{
    public class Channel
    {
        public int id { get; set; }
        private LinkedList<User> users = new LinkedList<User>();

        public Channel()
        {
            this.id = ChannelManager.GetInstance().RequestChannelID();
            ChannelManager.GetInstance().channels.AddLast(this);
        }

        public Channel(int id)
        {
            this.id = id;
            Console.Out.WriteLine("Creating channel with id " + id);
            ChannelManager.GetInstance().channels.AddLast(this);
        }

        /// <summary>
        /// Adds a user to the list, and notifies other users of this person's arrival.
        /// </summary>
        /// <param name="toAdd">The user to add to this channel</param>
        public void AddUser(User toAdd)
        {
            users.AddLast(toAdd);
            for (int i = 0; i < users.Count; i++)
            {
                User user = users.ElementAt(i);
                Packet p = new Packet(Headers.NEW_USER);
                p.AddInt(toAdd.id);
                p.AddString(toAdd.username);
                user.client.client.SendPacket(p);
            }
        }

        /// <summary>
        /// A user has left this channel.
        /// </summary>
        /// <param name="toRemove">The user to remove from this channel</param>
        public void UserLeft(User toRemove)
        {
            users.Remove(toRemove);
            Console.Out.WriteLine("Removed a user.");
            for (int i = 0; i < users.Count; i++)
            {
                User user = users.ElementAt(i);
                Packet p = new Packet(Headers.USER_LEFT);
                p.AddInt(toRemove.id);
                p.AddString(toRemove.username);
                user.client.client.SendPacket(p);
            }
        }

        /// <summary>
        /// Sends a message to all users in this channel.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void SendMessageToUsers(String message)
        {
            for (int i = 0; i < users.Count; i++)
            {
                User user = users.ElementAt(i);
                Packet p = new Packet(Headers.CHAT_MESSAGE);
                p.AddInt(id);
                p.AddString(message);
                user.client.client.SendPacket(p);
            }
        }
    }
}
