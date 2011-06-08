using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SocketLibrary;
using System.Threading;
using GameServer.ChatServer;
using GameServer.GameServer;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using GameServer.ChatServer.Channels;

namespace GameServer
{
    public partial class ServerUI : Form
    {
        private static ServerUI instance { get; set; }

        private ServerUI()
        {
            InitializeComponent();
            ClientsListView.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(ClientsSelectionChanged);

            this.Disposed += new EventHandler(ServerUI_Disposed);

            // Create a new channel. All is done in the constructor
            new Channel();
        }

        public static ServerUI GetInstance()
        {
            if (instance == null) instance = new ServerUI();
            return instance;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Messages_Click(object sender, EventArgs e)
        {

        }


        private void ServerUI_Load(object sender, EventArgs e)
        {

        }

        private void StartGameServer_Click(object sender, EventArgs e)
        {
            GameServerManager.GetInstance().Start();
        }

        private void StartChatServer_Click(object sender, EventArgs e)
        {
            ChatServerManager.GetInstance().Start();
        }

        private void StopGameServer_Click(object sender, EventArgs e)
        {
            GameServerManager.GetInstance().Stop();
        }

        private void StopChatServer_Click(object sender, EventArgs e)
        {
            ChatServerManager.GetInstance().Stop();
        }

        private void ViewGameServerClientsBtn_Click(object sender, EventArgs e)
        {
            ClientsListView.Items.Clear();
            MessagesListView.Items.Clear();
            RemoveDisposedConnections();
            foreach (SocketClient client in GameServerManager.GetInstance().clients)
            {
                ClientsListView.Items.Add(new ListViewItem(client.Sock.RemoteEndPoint.ToString(), 0));
            }
        }

        private void ViewChatServerClientsBtn_Click(object sender, EventArgs e)
        {
            ClientsListView.Items.Clear();
            MessagesListView.Items.Clear();
            RemoveDisposedConnections();
            foreach (ChatClientListener clientListener in ChatServerManager.GetInstance().clients)
            {
                ClientsListView.Items.Add(new ListViewItem(clientListener.client.GetRemoteHostIP()));
            }
        }

        void ClientsSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            this.MessagesListView.Items.Clear();
            if (e.IsSelected)
            {
                foreach (ChatClientListener clientListener in ChatServerManager.GetInstance().clients)
                {
                    Console.Out.WriteLine("Comparing " + e.Item.Text + " to " + clientListener.client.GetRemoteHostIP());
                    if (e.Item.Text == clientListener.client.GetRemoteHostIP())
                    {
                        foreach (String s in clientListener.client.messageLog)
                        {
                            this.MessagesListView.Items.Add(new ListViewItem(s));
                        }
                        break;
                    }
                }
            }
        }

        private void RemoveDisposedConnections()
        {
            // TODO: Remove disposed game connections

            // /TODO
            // Remove disposed connections
            for (int i = 0; i < ChatServerManager.GetInstance().clients.Count; i++)
            {
                ChatClientListener cl = ChatServerManager.GetInstance().clients.ElementAt(i);
                if (cl.client.GetRemoteHostIP() == "SOCKET DISPOSED")
                {
                    ChatServerManager.GetInstance().clients.Remove(cl);
                    i--;
                }
            }
        }

        private void ServerUI_Disposed(object sender, EventArgs e)
        {
            RemoveDisposedConnections();
            ChatServerManager.GetInstance().Stop();
        }
    }
}
