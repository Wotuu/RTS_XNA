using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary;
using System.Net.Sockets;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using System.Threading;
using PathfindingTest.UI.Menus;
using PathfindingTest.UI.Menus.Multiplayer;
using XNAInterfaceComponents.AbstractComponents;
using XNAInterfaceComponents.ParentComponents;
using SocketLibrary.Users;
using SocketLibrary.Multiplayer;

namespace PathfindingTest.Multiplayer.SocketConnection
{
    public class ChatServerConnectionManager
    {
        private static ChatServerConnectionManager instance { get; set; }
        public SocketClient connection { get; set; }

        public String serverLocation = "localhost";
        private int serverPort = 14051;

        public User user { get; set; }
        public Boolean safeShutdown { get; set; }

        private ChatPacketProcessor chatPacketProcessor = new ChatPacketProcessor();
        private GameLobbyPacketProcessor gameLobbyPacketProcessor = new GameLobbyPacketProcessor();

        private ChatServerConnectionManager()
        {
        }

        /// <summary>
        /// The server disconnected :(
        /// </summary>
        public void OnDisconnect()
        {
            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MultiplayerLogin);
            if (!safeShutdown)
            {
                XNAMessageDialog.CreateDialog(
                    "The server disconnected.", XNAMessageDialog.DialogType.OK)
                    .button1.onClickListeners += ServerDisconnectOKButtonPressed;
            }
            safeShutdown = false;
        }

        public static ChatServerConnectionManager GetInstance()
        {
            if (instance == null) instance = new ChatServerConnectionManager();
            return instance;
        }

        /// <summary>
        /// Sets the status of the login label.
        /// </summary>
        /// <param name="s">The new status</param>
        public void SetLoginStatus(String s)
        {
            ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
            if (menu != null && menu is LoginScreen)
            {
                LoginScreen screen = ((LoginScreen)menu);
                screen.SetConnectionStatus(s);
            }
        }

        /// <summary>
        /// Connect to the server
        /// </summary>
        public void ConnectToServer()
        {
            TcpClient sock = new TcpClient();
            try
            {
                SetLoginStatus("Connecting..");
                sock.Connect(this.serverLocation, this.serverPort);
            }
            catch (SocketException e)
            {
                XNAMessageDialog.CreateDialog(
                    "The server is offline.", XNAMessageDialog.DialogType.OK);
                SetLoginStatus("Server is offline. Try again.");
                return;
            }

            this.connection = new SocketClient(sock.Client, "rts_client");
            new Thread(this.connection.Enable).Start();

            // Give the above thread some time to start running! 
            // Else we'll get a nullpointer right here.
            Thread.Sleep(50);
            this.connection.packetProcessor.onProcessPacket += chatPacketProcessor.DataReceived;
            this.connection.packetProcessor.onProcessPacket += gameLobbyPacketProcessor.DataReceived;
            this.connection.onDisconnectListeners += this.OnDisconnect;

            SetLoginStatus("Handshaking..");
            this.SendPacket(new Packet(Headers.HANDSHAKE_1));
        }


        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        public void DisconnectFromServer()
        {
            if (this.connection != null && !this.connection.Receiving) return;
            this.SendPacket(new Packet(Headers.CLIENT_DISCONNECT));
            this.connection.Disable();
            this.safeShutdown = true;
        }

        /// <summary>
        /// Called when the client received data from the server.
        /// </summary>
        /// <param name="p">The packet that was received</param>
        /*public void DataReceived(Packet p)
        {
            switch (p.GetHeader())
            {
                case Headers.KEEP_ALIVE:
                    {
                        this.SendPacket(new Packet(Headers.KEEP_ALIVE));
                        break;
                    }
                case Headers.HANDSHAKE_2:
                    {
                        SetLoginStatus("Connected");
                        // Finish handshake
                        this.SendPacket(new Packet(Headers.HANDSHAKE_3));

                        // Request for a user ID
                        Packet newPacket = new Packet(Headers.CLIENT_USERNAME);
                        newPacket.AddString(this.user.username);
                        this.SendPacket(newPacket);

                        MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MultiplayerLobby);
                        break;
                    }
                case Headers.SERVER_DISCONNECT:
                    {
                        this.DisconnectFromServer();
                        // Create a dialog, and add a listener to the OK button.
                        this.OnDisconnect();
                        break;
                    }
                case Headers.CLIENT_USER_ID:
                    {
                        // Set the received user ID.
                        this.user.id = PacketUtil.DecodePacketInt(p, 0);
                        // Console.Out.WriteLine("Received user ID from the server: " + this.user.id);
                        break;
                    }
                case Headers.CLIENT_CHANNEL:
                    {
                        this.user.channelID = PacketUtil.DecodePacketInt(p, 0);
                        Console.Out.WriteLine("Switched channel to: " + this.user.channelID);
                        break;
                    }
                case Headers.CHAT_MESSAGE:
                    {
                        // Get the channel
                        int channel = PacketUtil.DecodePacketInt(p, 0);
                        // Get the message
                        String message = PacketUtil.DecodePacketString(p, 4);
                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (menu is MultiplayerLobby)
                        {
                            MultiplayerLobby lobby = ((MultiplayerLobby)menu);
                            lobby.AddMessageToLog(message);
                        }
                        else if (menu is GameLobby)
                        {
                            GameLobby lobby = ((GameLobby)menu);
                            lobby.AddMessageToLog(message);
                        }
                        break;
                    }
                case Headers.NEW_USER:
                    {
                        int userID = PacketUtil.DecodePacketInt(p, 0);
                        String username = PacketUtil.DecodePacketString(p, 4);
                        User user = new User(username);
                        user.id = userID;
                        user.channelID = this.user.channelID;
                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();

                        if (menu is MultiplayerLobby)
                        {
                            MultiplayerLobby lobby = ((MultiplayerLobby)menu);
                            lobby.AddUser(user);
                        }
                        else if (menu is GameLobby)
                        {
                            GameLobby lobby = ((GameLobby)menu);
                            lobby.UserJoined(user);
                            Console.Out.WriteLine("User has joined the game lobby!");
                        }

                        break;
                    }
                case Headers.USER_LEFT:
                    {
                        int userID = PacketUtil.DecodePacketInt(p, 0);
                        User user = UserManager.GetInstance().GetUserByID(userID);
                        if (user != null)
                        {
                            ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                            if (menu is MultiplayerLobby)
                            {
                                MultiplayerLobby lobby = ((MultiplayerLobby)menu);
                                lobby.RemoveUser(user);
                            }
                            else if (menu is GameLobby)
                            {
                                GameLobby lobby = ((GameLobby)menu);
                                lobby.UserLeft(user);
                                Console.Out.WriteLine("User has left the game lobby!");
                            }
                        }

                        break;
                    }
                
            }
        }*/

        /// <summary>
        /// User accepted disconnect by the server.
        /// </summary>
        /// <param name="source"></param>
        public void ServerDisconnectOKButtonPressed(XNAButton source)
        {
            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MultiplayerLogin);
        }

        /// <summary>
        /// Attempts to send a packet to the server.
        /// </summary>
        /// <returns>If the packet was sent or not.</returns>
        /// <param name="packet">The packet to send</param>
        public Boolean SendPacket(Packet packet)
        {
            if (packet.GetFullData().Length == 0)
            {
                Console.Error.WriteLine("Cannot send a packet with length = 0");
                return false;
            }
            this.connection.SendPacket(packet);
            return true;
        }
    }
}
