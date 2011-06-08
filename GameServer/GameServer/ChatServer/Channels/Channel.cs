using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Users;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using SocketLibrary.Multiplayer;

namespace GameServer.ChatServer.Channels
{
    public class Channel
    {
        public int id { get; set; }
        private LinkedList<ServerUser> users = new LinkedList<ServerUser>();

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
        public void AddUser(ServerUser toAdd)
        {
            Console.Out.WriteLine("Channel " + this.id + " adds a user: " + toAdd);
            toAdd.channelID = this.id;
            // The user that joined must know who is already in the channel
            for (int i = 0; i < users.Count; i++)
            {
                ServerUser user = users.ElementAt(i);
                Packet p = new Packet(Headers.NEW_USER);
                p.AddInt(user.id);
                p.AddString(user.username);
                toAdd.client.client.SendPacket(p);
            }
            // User joins the channel
            users.AddLast(toAdd);
            Packet newChannelPacket = new Packet(Headers.CLIENT_CHANNEL);
            newChannelPacket.AddInt(this.id);
            toAdd.client.client.SendPacket(newChannelPacket);

            // Notify everyone that this user has joined (including our new user)
            for (int i = 0; i < users.Count; i++)
            {
                ServerUser user = users.ElementAt(i);
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
        public void UserLeft(ServerUser toRemove)
        {
            Console.Out.Write("Removed a user from channel " + this.id + ". Old count: " + users.Count);
            users.Remove(toRemove);
            for (int i = 0; i < users.Count; i++)
            {
                ServerUser user = users.ElementAt(i);
                Packet p = new Packet(Headers.USER_LEFT);
                p.AddInt(toRemove.id);
                p.AddString(toRemove.username);
                user.client.client.SendPacket(p);
            }
            Console.Out.WriteLine(", new count: " + users.Count);
        }

        /// <summary>
        /// Sends a message to all users in this channel.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void SendMessageToUsers(String message)
        {
            for (int i = 0; i < users.Count; i++)
            {
                ServerUser user = users.ElementAt(i);
                Packet p = new Packet(Headers.CHAT_MESSAGE);
                p.AddInt(id);
                p.AddString(message);
                user.client.client.SendPacket(p);
            }
        }

        /// <summary>
        /// Notify all users in this channel of a game that has been created.
        /// </summary>
        /// <param name="game"></param>
        public void CreatedGame(MultiplayerGame game)
        {
            if (this.id != 1)
            {
                Console.Error.WriteLine("Attempted to create a game in channel " + this.id + ". This is not allowed. Fix this message.");
                return;
            }
            // Host should no longer be in the lobby
            this.UserLeft((ServerUser)game.host);
            for (int i = 0; i < users.Count; i++)
            {
                ServerUser user = users.ElementAt(i);
                Packet p = new Packet(Headers.SERVER_CREATE_GAME);
                p.AddInt(game.id);
                p.AddInt(game.host.id);
                p.AddString(game.gamename);
                user.client.client.SendPacket(p);
            }
        }

        /// <summary>
        /// Sends a packet to all users in this channel.
        /// </summary>
        /// <param name="p">The packet to send.</param>
        public void SendPacketToAll(Packet p)
        {
            for (int i = 0; i < users.Count; i++)
            {
                users.ElementAt(i).client.client.SendPacket(p);
            }
        }
    }
}
