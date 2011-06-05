using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary;
using SocketLibrary.Protocol;
using SocketLibrary.Packets;
using GameServer.Users;
using GameServer.ChatServer.Channels;

namespace GameServer.ChatServer
{
    public class ChatClientListener
    {
        public SocketClient client { get; set; }
        public long lastAliveTicks { get; set; }

        public ChatClientListener(SocketClient client)
        {
            this.client = client;

            client.onPacketReceivedListeners += this.OnPacketReceived;
        }

        /// <summary>
        /// Called when the server recieved data from the client.
        /// </summary>
        /// <param name="data">The data that was sent</param>
        public void OnPacketReceived(Packet p)
        {
            switch (p.GetHeader())
            {
                case Headers.KEEP_ALIVE:
                    {
                        lastAliveTicks = System.DateTime.UtcNow.Ticks;
                        break;
                    }
                case Headers.HANDSHAKE_1:
                    {
                        client.SendPacket(new Packet(Headers.HANDSHAKE_2));
                        break;
                    }
                case Headers.CLIENT_DISCONNECT:
                    {
                        this.client.Disable();
                        ChatServerManager.GetInstance().clients.Remove(this);
                        break;
                    }
                case Headers.CLIENT_USERNAME:
                    {
                        User user = new User(PacketUtil.DecodePacketString(p, 0), this);

                        Packet newPacket = new Packet(Headers.CLIENT_USER_ID);
                        newPacket.AddInt(user.id);
                        this.client.SendPacket(newPacket);

                        // Put the user in channel 1 (the main lobby)
                        ChannelManager.GetInstance().GetChannelByID(1).AddUser(user);

                        break;
                    }
                case Headers.CHAT_MESSAGE:
                    {
                        // Get the channel
                        int channel = PacketUtil.DecodePacketInt(p, 0);
                        // Get the message
                        String message = PacketUtil.DecodePacketString(p, 4);
                        // Send it to the users!
                        ChannelManager.GetInstance().GetChannelByID(channel).SendMessageToUsers(message);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}
