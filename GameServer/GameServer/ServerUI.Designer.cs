namespace GameServer
{
    partial class ServerUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ServerMenuStrip = new System.Windows.Forms.MenuStrip();
            this.ServerMenuTab = new System.Windows.Forms.ToolStripMenuItem();
            this.StartServerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StartGameServer = new System.Windows.Forms.ToolStripMenuItem();
            this.StartChatServer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.StopServerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StopGameServer = new System.Windows.Forms.ToolStripMenuItem();
            this.StopChatServer = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ClientsListView = new System.Windows.Forms.ListView();
            this.IP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Username = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClientsLbl = new System.Windows.Forms.Label();
            this.ClientsPanel = new System.Windows.Forms.Panel();
            this.MessagesPanel = new System.Windows.Forms.Panel();
            this.MessagesListView = new System.Windows.Forms.ListView();
            this.MessagesLbl = new System.Windows.Forms.Label();
            this.ChatServerStatusLbl = new System.Windows.Forms.Label();
            this.ChatServerLbl = new System.Windows.Forms.Label();
            this.GameServerLabel = new System.Windows.Forms.Label();
            this.GameServerStatusLbl = new System.Windows.Forms.Label();
            this.ViewGameServerClientsBtn = new System.Windows.Forms.Button();
            this.ViewChatServerClientsBtn = new System.Windows.Forms.Button();
            this.ServerMenuStrip.SuspendLayout();
            this.ClientsPanel.SuspendLayout();
            this.MessagesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ServerMenuStrip
            // 
            this.ServerMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ServerMenuTab});
            this.ServerMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.ServerMenuStrip.Name = "ServerMenuStrip";
            this.ServerMenuStrip.Size = new System.Drawing.Size(784, 24);
            this.ServerMenuStrip.TabIndex = 0;
            this.ServerMenuStrip.Text = "menuStrip1";
            // 
            // ServerMenuTab
            // 
            this.ServerMenuTab.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StartServerMenuItem,
            this.toolStripSeparator2,
            this.StopServerMenuItem,
            this.ExitMenuItem});
            this.ServerMenuTab.Name = "ServerMenuTab";
            this.ServerMenuTab.Size = new System.Drawing.Size(51, 20);
            this.ServerMenuTab.Text = "Server";
            this.ServerMenuTab.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // StartServerMenuItem
            // 
            this.StartServerMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StartGameServer,
            this.StartChatServer});
            this.StartServerMenuItem.Name = "StartServerMenuItem";
            this.StartServerMenuItem.Size = new System.Drawing.Size(133, 22);
            this.StartServerMenuItem.Text = "Start Server";
            // 
            // StartGameServer
            // 
            this.StartGameServer.Name = "StartGameServer";
            this.StartGameServer.Size = new System.Drawing.Size(140, 22);
            this.StartGameServer.Text = "Game Server";
            this.StartGameServer.Click += new System.EventHandler(this.StartGameServer_Click);
            // 
            // StartChatServer
            // 
            this.StartChatServer.Name = "StartChatServer";
            this.StartChatServer.Size = new System.Drawing.Size(140, 22);
            this.StartChatServer.Text = "Chat Server";
            this.StartChatServer.Click += new System.EventHandler(this.StartChatServer_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(130, 6);
            // 
            // StopServerMenuItem
            // 
            this.StopServerMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StopGameServer,
            this.StopChatServer});
            this.StopServerMenuItem.Name = "StopServerMenuItem";
            this.StopServerMenuItem.Size = new System.Drawing.Size(133, 22);
            this.StopServerMenuItem.Text = "Stop Server";
            // 
            // StopGameServer
            // 
            this.StopGameServer.Enabled = false;
            this.StopGameServer.Name = "StopGameServer";
            this.StopGameServer.Size = new System.Drawing.Size(140, 22);
            this.StopGameServer.Text = "Game Server";
            this.StopGameServer.Click += new System.EventHandler(this.StopGameServer_Click);
            // 
            // StopChatServer
            // 
            this.StopChatServer.Enabled = false;
            this.StopChatServer.Name = "StopChatServer";
            this.StopChatServer.Size = new System.Drawing.Size(140, 22);
            this.StopChatServer.Text = "Chat Server";
            this.StopChatServer.Click += new System.EventHandler(this.StopChatServer_Click);
            // 
            // ExitMenuItem
            // 
            this.ExitMenuItem.Name = "ExitMenuItem";
            this.ExitMenuItem.Size = new System.Drawing.Size(133, 22);
            this.ExitMenuItem.Text = "Exit";
            this.ExitMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // ClientsListView
            // 
            this.ClientsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IP,
            this.Username});
            this.ClientsListView.Location = new System.Drawing.Point(3, 23);
            this.ClientsListView.Name = "ClientsListView";
            this.ClientsListView.Size = new System.Drawing.Size(300, 184);
            this.ClientsListView.TabIndex = 1;
            this.ClientsListView.UseCompatibleStateImageBehavior = false;
            this.ClientsListView.View = System.Windows.Forms.View.List;
            // 
            // IP
            // 
            this.IP.Text = "IP";
            this.IP.Width = 300;
            // 
            // Username
            // 
            this.Username.Text = "Username";
            this.Username.Width = 150;
            // 
            // ClientsLbl
            // 
            this.ClientsLbl.AutoSize = true;
            this.ClientsLbl.Location = new System.Drawing.Point(3, 9);
            this.ClientsLbl.Name = "ClientsLbl";
            this.ClientsLbl.Size = new System.Drawing.Size(38, 13);
            this.ClientsLbl.TabIndex = 2;
            this.ClientsLbl.Text = "Clients";
            // 
            // ClientsPanel
            // 
            this.ClientsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ClientsPanel.Controls.Add(this.ClientsListView);
            this.ClientsPanel.Controls.Add(this.ClientsLbl);
            this.ClientsPanel.Location = new System.Drawing.Point(466, 38);
            this.ClientsPanel.Name = "ClientsPanel";
            this.ClientsPanel.Size = new System.Drawing.Size(306, 212);
            this.ClientsPanel.TabIndex = 3;
            // 
            // MessagesPanel
            // 
            this.MessagesPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MessagesPanel.Controls.Add(this.MessagesListView);
            this.MessagesPanel.Controls.Add(this.MessagesLbl);
            this.MessagesPanel.Location = new System.Drawing.Point(0, 251);
            this.MessagesPanel.Name = "MessagesPanel";
            this.MessagesPanel.Size = new System.Drawing.Size(772, 299);
            this.MessagesPanel.TabIndex = 4;
            // 
            // MessagesListView
            // 
            this.MessagesListView.Location = new System.Drawing.Point(3, 21);
            this.MessagesListView.Name = "MessagesListView";
            this.MessagesListView.Size = new System.Drawing.Size(764, 273);
            this.MessagesListView.TabIndex = 1;
            this.MessagesListView.UseCompatibleStateImageBehavior = false;
            this.MessagesListView.View = System.Windows.Forms.View.List;
            // 
            // MessagesLbl
            // 
            this.MessagesLbl.AutoSize = true;
            this.MessagesLbl.Location = new System.Drawing.Point(12, 4);
            this.MessagesLbl.Name = "MessagesLbl";
            this.MessagesLbl.Size = new System.Drawing.Size(55, 13);
            this.MessagesLbl.TabIndex = 0;
            this.MessagesLbl.Text = "Messages";
            this.MessagesLbl.Click += new System.EventHandler(this.Messages_Click);
            // 
            // ChatServerStatusLbl
            // 
            this.ChatServerStatusLbl.AutoSize = true;
            this.ChatServerStatusLbl.Location = new System.Drawing.Point(93, 64);
            this.ChatServerStatusLbl.Name = "ChatServerStatusLbl";
            this.ChatServerStatusLbl.Size = new System.Drawing.Size(69, 13);
            this.ChatServerStatusLbl.TabIndex = 5;
            this.ChatServerStatusLbl.Text = "Server offline";
            // 
            // ChatServerLbl
            // 
            this.ChatServerLbl.AutoSize = true;
            this.ChatServerLbl.Location = new System.Drawing.Point(15, 64);
            this.ChatServerLbl.Name = "ChatServerLbl";
            this.ChatServerLbl.Size = new System.Drawing.Size(66, 13);
            this.ChatServerLbl.TabIndex = 6;
            this.ChatServerLbl.Text = "Chat Server:";
            // 
            // GameServerLabel
            // 
            this.GameServerLabel.AutoSize = true;
            this.GameServerLabel.Location = new System.Drawing.Point(15, 38);
            this.GameServerLabel.Name = "GameServerLabel";
            this.GameServerLabel.Size = new System.Drawing.Size(72, 13);
            this.GameServerLabel.TabIndex = 7;
            this.GameServerLabel.Text = "Game Server:";
            // 
            // GameServerStatusLbl
            // 
            this.GameServerStatusLbl.AutoSize = true;
            this.GameServerStatusLbl.Location = new System.Drawing.Point(93, 38);
            this.GameServerStatusLbl.Name = "GameServerStatusLbl";
            this.GameServerStatusLbl.Size = new System.Drawing.Size(69, 13);
            this.GameServerStatusLbl.TabIndex = 8;
            this.GameServerStatusLbl.Text = "Server offline";
            // 
            // ViewGameServerClientsBtn
            // 
            this.ViewGameServerClientsBtn.Enabled = false;
            this.ViewGameServerClientsBtn.Location = new System.Drawing.Point(238, 33);
            this.ViewGameServerClientsBtn.Name = "ViewGameServerClientsBtn";
            this.ViewGameServerClientsBtn.Size = new System.Drawing.Size(75, 23);
            this.ViewGameServerClientsBtn.TabIndex = 9;
            this.ViewGameServerClientsBtn.Text = "View Clients";
            this.ViewGameServerClientsBtn.UseVisualStyleBackColor = true;
            this.ViewGameServerClientsBtn.Click += new System.EventHandler(this.ViewGameServerClientsBtn_Click);
            // 
            // ViewChatServerClientsBtn
            // 
            this.ViewChatServerClientsBtn.Enabled = false;
            this.ViewChatServerClientsBtn.Location = new System.Drawing.Point(238, 59);
            this.ViewChatServerClientsBtn.Name = "ViewChatServerClientsBtn";
            this.ViewChatServerClientsBtn.Size = new System.Drawing.Size(75, 23);
            this.ViewChatServerClientsBtn.TabIndex = 10;
            this.ViewChatServerClientsBtn.Text = "View Clients";
            this.ViewChatServerClientsBtn.UseVisualStyleBackColor = true;
            this.ViewChatServerClientsBtn.Click += new System.EventHandler(this.ViewChatServerClientsBtn_Click);
            // 
            // ServerUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.ViewChatServerClientsBtn);
            this.Controls.Add(this.ViewGameServerClientsBtn);
            this.Controls.Add(this.GameServerStatusLbl);
            this.Controls.Add(this.GameServerLabel);
            this.Controls.Add(this.ChatServerLbl);
            this.Controls.Add(this.ChatServerStatusLbl);
            this.Controls.Add(this.MessagesPanel);
            this.Controls.Add(this.ClientsPanel);
            this.Controls.Add(this.ServerMenuStrip);
            this.Name = "ServerUI";
            this.Text = "XNA RTS Server";
            this.Load += new System.EventHandler(this.ServerUI_Load);
            this.ServerMenuStrip.ResumeLayout(false);
            this.ServerMenuStrip.PerformLayout();
            this.ClientsPanel.ResumeLayout(false);
            this.ClientsPanel.PerformLayout();
            this.MessagesPanel.ResumeLayout(false);
            this.MessagesPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip ServerMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem ServerMenuTab;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
        private System.Windows.Forms.Label ClientsLbl;
        private System.Windows.Forms.Panel ClientsPanel;
        private System.Windows.Forms.Panel MessagesPanel;
        private System.Windows.Forms.Label MessagesLbl;
        public System.Windows.Forms.ListView ClientsListView;
        public System.Windows.Forms.ListView MessagesListView;
        public System.Windows.Forms.Label ChatServerStatusLbl;
        private System.Windows.Forms.Label ChatServerLbl;
        private System.Windows.Forms.Label GameServerLabel;
        private System.Windows.Forms.ToolStripMenuItem StartServerMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StopServerMenuItem;
        public System.Windows.Forms.ToolStripMenuItem StartGameServer;
        public System.Windows.Forms.ToolStripMenuItem StartChatServer;
        public System.Windows.Forms.ToolStripMenuItem StopGameServer;
        public System.Windows.Forms.ToolStripMenuItem StopChatServer;
        public System.Windows.Forms.Label GameServerStatusLbl;
        public System.Windows.Forms.Button ViewGameServerClientsBtn;
        public System.Windows.Forms.Button ViewChatServerClientsBtn;
        private System.Windows.Forms.ColumnHeader IP;
        private System.Windows.Forms.ColumnHeader Username;
    }
}

