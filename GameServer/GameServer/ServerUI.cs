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
using System.Runtime.InteropServices;

namespace GameServer
{
    public partial class ServerUI : Form
    {
        private static ServerUI instance { get; set; }
        public String lastSelectedClientName { get; set; }
        #region Scrollbar crap
        [DllImport("user32.dll")]
        static public extern bool ShowScrollBar(System.IntPtr hWnd, int wBar, bool bShow);
        private const uint SB_HORZ = 0;
        private const uint SB_VERT = 1;
        private const uint ESB_DISABLE_BOTH = 0x3;
        private const uint ESB_ENABLE_BOTH = 0x0;
        #endregion

        private ServerUI()
        {
            InitializeComponent();
            ClientsListView.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(ClientsSelectionChanged);

            this.Disposed += new EventHandler(ServerUI_Disposed);
            // ShowScrollBar(this.ReceivedMessagesListView.Handle, (int)SB_VERT, true);
            // ShowScrollBar(this.SentMessagesListView.Handle, (int)SB_VERT, true);
            // Create a new channel (the lobby!). All is done in the constructor
            new Channel();
        }

        public static ServerUI GetInstance()
        {
            if (instance == null) instance = new ServerUI();
            return instance;
        }

        /// <summary>
        /// Gets the selected client name.
        /// </summary>
        /// <returns>The selected client name.</returns>
        public String GetSelectedClientName()
        {
            if (this.ClientsListView.SelectedItems.Count == 0) return "auriaejwfiuhef";
            return this.ClientsListView.SelectedItems[0].ToString();
        }

        /// <summary>
        /// Refills the message logs.
        /// </summary>
        /// <param name="listener">The listener to display.</param>
        public void RefillMessageLogs(ChatClientListener listener)
        {
            this.ReceivedMessagesListView.Items.Clear();
            this.SentMessagesListView.Items.Clear();

            for (int i = 0; i < listener.client.messageLog.Count; i++)
            {
                SocketLibrary.SocketClient.LogMessage message = listener.client.messageLog.ElementAt(i);
                String[] split = message.message.Split(' ');
                String msg = "";
                for (int j = 2; j < split.Length; j++)
                {
                    msg += split[j] + " ";
                }
                if (message.received) this.ReceivedMessagesListView.Items.Add(
                    new ListViewItem(new String[] { split[0], split[1], msg }));
                else this.SentMessagesListView.Items.Add(
                    new ListViewItem(new String[] { split[0], split[1], msg }));
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
            SentMessagesListView.Items.Clear();
            ReceivedMessagesListView.Items.Clear();
            RemoveDisposedConnections();
            foreach (SocketClient client in GameServerManager.GetInstance().clients)
            {
                ClientsListView.Items.Add(new ListViewItem(client.Sock.RemoteEndPoint.ToString(), 0));
            }
        }

        private void ViewChatServerClientsBtn_Click(object sender, EventArgs e)
        {
            ClientsListView.Items.Clear();
            ReceivedMessagesListView.Items.Clear();
            SentMessagesListView.Items.Clear();
            RemoveDisposedConnections();
            foreach (ChatClientListener clientListener in ChatServerManager.GetInstance().clients)
            {
                ClientsListView.Items.Add(new ListViewItem(
                    new String[] { clientListener.user.username, clientListener.client.GetRemoteHostIP() }));
            }
        }


        void ClientsSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            this.ReceivedMessagesListView.Items.Clear();
            this.SentMessagesListView.Items.Clear();
            if (e.IsSelected)
            {
                foreach (ChatClientListener clientListener in ChatServerManager.GetInstance().clients)
                {
                    // Console.Out.WriteLine("Comparing " + e.Item.Text + " to " + clientListener.client.GetRemoteHostIP());
                    if (e.Item.Text == clientListener.user.username)
                    {
                        this.lastSelectedClientName = clientListener.user.username;
                        RefillMessageLogs(clientListener);
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
