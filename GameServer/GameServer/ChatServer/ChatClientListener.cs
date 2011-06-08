using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary;
using SocketLibrary.Protocol;
using SocketLibrary.Packets;
using GameServer.Users;
using GameServer.ChatServer.Channels;
using System.Threading;
using GameServer.ChatServer.Games;
using SocketLibrary.Multiplayer;

namespace GameServer.ChatServer
{
    public class ChatClientListener
    {
        public SocketClient client { get; set; }
        public long lastAliveTicks { get; set; }
        public Boolean safeShutDown { get; set; }
        public ServerUser user { get; set; }

        public ChatClientListener(SocketClient client)
        {
            this.client = client;
            // Wait for it ...
            while (client.packetProcessor == null) { Console.Out.WriteLine("Waiting for instance.."); Thread.Sleep(1); }
            client.packetProcessor.onProcessPacket += this.OnPacketReceived;
            this.client.onDisconnectListeners += this.OnDisconnect;
        }

        /// <summary>
        /// When the user disconnected
        /// </summary>
        public void OnDisconnect()
        {
            if (ChatServerManager.GetInstance().clients == null) Console.Out.WriteLine("Clients was null :O");
            Console.Out.Write("Client destroyed! -> " + ChatServerManager.GetInstance().clients.Count);
            if (!safeShutDown)
            {
                ChatServerManager.GetInstance().clients.Remove(this);
                this.client.Disable();
            }
            Console.Out.WriteLine(", " + ChatServerManager.GetInstance().clients.Count);
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
                        this.OnDisconnect();
                        break;
                    }
                case Headers.CLIENT_USERNAME:
                    {
                        this.user = new ServerUser(PacketUtil.DecodePacketString(p, 0), this);

                        Packet newPacket = new Packet(Headers.CLIENT_USER_ID);
                        newPacket.AddInt(this.user.id);
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
                case Headers.CLIENT_CREATE_GAME:
                    {
                        if( this.user.channelID != 1 ) {
                            Console.Error.WriteLine("Received a request to create a channel that is NOT from a user in the lobby!");
                            return;
                        }
                        // User that requested this
                        int userID = PacketUtil.DecodePacketInt(p, 0);
                        // Name of the game
                        String gameName = PacketUtil.DecodePacketString(p, 4);
                        // Create game
                        MultiplayerGame mg = new MultiplayerGame(
                            MultiplayerGameManager.GetInstance().RequestGameID(),
                            (ServerUser) ServerUserManager.GetInstance().GetUserByID(userID),
                            gameName, "<No map selected yet>");
                        // Add game to list
                        MultiplayerGameManager.GetInstance().games.AddLast(mg);
                        Console.Out.WriteLine("Created a game with ID = " + mg.id);

                        // Return the host its channel and game ID
                        Packet gameIDPacket = new Packet(Headers.GAME_ID);
                        gameIDPacket.AddInt(mg.id);
                        this.client.SendPacket(gameIDPacket);

                        this.user.ChangeChannel(mg.id);

                        // Notify all others in the channel that the game was created.
                        ChannelManager.GetInstance().GetChannelByID(1).CreatedGame(mg);
                        break;
                    }
                case Headers.GAME_MAP_CHANGED:
                    {

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
