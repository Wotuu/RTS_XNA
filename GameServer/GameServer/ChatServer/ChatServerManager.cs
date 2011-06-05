using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary;
using System.Threading;

namespace GameServer.ChatServer
{
    public class ChatServerManager
    {
        private static ChatServerManager instance { get; set; }
        public SocketServer serverSocket { get; set; }
        public int port = 14051;
        public LinkedList<ChatClientListener> clients { get; set; }

        public static ChatServerManager GetInstance()
        {
            if (instance == null) instance = new ChatServerManager();
            return instance;
        }

        private ChatServerManager() {
            clients = new LinkedList<ChatClientListener>();
        }

        public void Start() {
            Console.Out.WriteLine("Chat server started on port " + this.port);
            SocketServer socket = new SocketServer(port, false, "rts_xna_chatserver");
            this.serverSocket = socket;
            this.serverSocket.onClientConnectedListeners += this.OnClientConnected;
            new Thread(socket.Enable).Start();

            ServerUI.GetInstance().ChatServerStatusLbl.Text = "Server running @ " + this.port;
            ServerUI.GetInstance().StartChatServer.Enabled = false;
            ServerUI.GetInstance().StopChatServer.Enabled = true;
            ServerUI.GetInstance().ViewChatServerClientsBtn.Enabled = true;

        }

        public void Stop()
        {
            this.serverSocket.Disable();
            ServerUI.GetInstance().ChatServerStatusLbl.Text = "Server offline";
            ServerUI.GetInstance().StartChatServer.Enabled = true;
            ServerUI.GetInstance().StopChatServer.Enabled = false;
            ServerUI.GetInstance().ViewChatServerClientsBtn.Enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public void OnClientConnected(SocketClient client)
        {
            Console.Out.WriteLine("Client connected!");
            clients.AddLast(new ChatClientListener(client));
        }
    }
}
