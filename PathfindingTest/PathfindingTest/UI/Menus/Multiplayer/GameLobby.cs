using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.AbstractComponents;
using XNAInterfaceComponents.ChildComponents;
using XNAInputLibrary.KeyboardInput;
using PathfindingTest.Multiplayer.SocketConnection;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using PathfindingTest.Multiplayer;
using SocketLibrary.Multiplayer;
using PathfindingTest.UI.Menus.Multiplayer.Panels;
using SocketLibrary.Users;
using PathfindingTest.UI.Menus.Multiplayer.Misc;

namespace PathfindingTest.UI.Menus.Multiplayer
{
    public class GameLobby : XNAPanel
    {

        private LinkedList<Message> messageLog = new LinkedList<Message>();

        private XNATextField messagesTextField { get; set; }
        private XNATextField messageTextField { get; set; }
        public MultiplayerGame multiplayerGame { get; set; }

        private XNAPanel gameOptionsPanel { get; set; }
        private LinkedList<UserDisplayPanel> userDisplayPanels = new LinkedList<UserDisplayPanel>();

        public GameLobby()
            : base(null,
                new Rectangle(
                Game1.GetInstance().graphics.PreferredBackBufferWidth / 2 - 400,
                Game1.GetInstance().graphics.PreferredBackBufferHeight / 2 - 300,
                800, 600))
        {
            gameOptionsPanel = new XNAPanel(this, new Rectangle(5, 5, 500, 330));
            gameOptionsPanel.border = new Border(gameOptionsPanel, 1, Color.Blue);

            XNAPanel mapPreviewPanel = new XNAPanel(this, new Rectangle(510, 5, 200, 200));
            mapPreviewPanel.border = new Border(gameOptionsPanel, 1, Color.Blue);


            XNAPanel messagesPanel = new XNAPanel(this, new Rectangle(5, 340, 790, 210));
            messagesPanel.border = new Border(messagesPanel, 1, Color.Blue);

            messagesTextField = new XNATextField(messagesPanel, new Rectangle(5, 5, 780, 170), Int32.MaxValue);
            messagesTextField.border = new Border(messagesTextField, 1, Color.Black);
            messagesTextField.font = MenuManager.SMALL_TEXTFIELD_FONT;
            messagesTextField.isEditable = false;

            messageTextField = new XNATextField(messagesPanel, new Rectangle(5, 180, 780, 25), 1);
            messageTextField.border = new Border(messageTextField, 1, Color.Black);
            messageTextField.font = MenuManager.SMALL_TEXTFIELD_FONT;
            messageTextField.onTextFieldKeyPressedListeners += this.OnKeyPressed;

            XNAButton createGameButton = new XNAButton(this,
                new Rectangle(this.bounds.Width - 105, this.bounds.Height - 45, 100, 40), "Create Game");
            createGameButton.onClickListeners += StartGame;

            XNAButton leaveGameButton = new XNAButton(this,
                new Rectangle(5, this.bounds.Height - 45, 100, 40), "Leave Game");
            leaveGameButton.onClickListeners += LeaveGame;
        }

        /// <summary>
        /// Checks whether the user exists in this lobby.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public Boolean UserExists(int userID)
        {
            return UserExists(UserManager.GetInstance().GetUserByID(userID));
        }

        /// <summary>
        /// Checks whether the user exists in this lobby.
        /// </summary>
        /// <param name="user">The user to check</param>
        /// <returns></returns>
        public Boolean UserExists(User user)
        {
            foreach (UserDisplayPanel p in userDisplayPanels)
            {
                if (p.user == user) return true;
            }
            return false;
        }

        /// <summary>
        /// When a player joined, call this one.
        /// </summary>
        /// <param name="user">The user that joined.</param>
        public void UserJoined(User user)
        {
            // I prefer the ID function ..
            if (!UserExists(user.id))
            {
                Console.Out.WriteLine(user + " has joined the game lobby (from " + ChatServerConnectionManager.GetInstance().user + ")");
                userDisplayPanels.AddLast(new UserDisplayPanel(gameOptionsPanel, user, this.userDisplayPanels.Count));
            }
        }

        /// <summary>
        /// Call this when a player left.
        /// </summary>
        /// <param name="user">The user that left.</param>
        public void UserLeft(User user)
        {
            Console.Out.WriteLine(user + " has left the game lobby (from " + ChatServerConnectionManager.GetInstance().user + ")");
            Boolean removed = false;
            for (int i = 0; i < userDisplayPanels.Count; i++)
            {
                UserDisplayPanel p = userDisplayPanels.ElementAt(i);
                if (removed)
                {
                    p.UpdateBounds(i);
                }
                else if (p.user.id == user.id)
                {
                    userDisplayPanels.Remove(p);
                    p.Unload();
                    i--;
                    removed = true;
                }
            }
        }

        /// <summary>
        /// Whether the game is full or not.
        /// </summary>
        /// <returns></returns>
        public Boolean IsFull()
        {
            // TO-DO
            return false;
        }


        /// <summary>
        /// Pressed the start game button!
        /// </summary>
        /// <param name="source"></param>
        public void StartGame(XNAButton source)
        {

        }

        /// <summary>
        /// Leave the game.
        /// </summary>
        /// <param name="source">The button source</param>
        public void LeaveGame(XNAButton source)
        {
            // If I was the host..
            if (this.multiplayerGame != null)
            {
                // Destroy the game
                Packet destroyGame = new Packet(Headers.CLIENT_DESTROY_GAME);
                ChatServerConnectionManager.GetInstance().SendPacket(destroyGame);
            }

            // Change channel
            Packet leftGamePacket = new Packet(Headers.CLIENT_LEFT_GAME);
            ChatServerConnectionManager.GetInstance().SendPacket(leftGamePacket);

            // Show the menu
            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MultiplayerLobby);
        }

        /// <summary>
        /// User pressed a key in the message textfield.
        /// </summary>
        /// <param name="e">The event</param>
        public void OnKeyPressed(KeyEvent e)
        {
            if (e.key.ToString() == "Enter")
            {
                ChatServerConnectionManager chat = ChatServerConnectionManager.GetInstance();
                String message = chat.user.username + ": " + messageTextField.text;
                messageTextField.text = "";
                // AddMessageToLog(message);
                Packet packet = new Packet();

                packet.SetHeader(Headers.CHAT_MESSAGE);
                packet.AddInt(chat.user.channelID);
                packet.AddString(message);
                chat.SendPacket(packet);
            }
        }

        /// <summary>
        /// Adds a message to the log.
        /// </summary>
        /// <param name="message">The message to add</param>
        public void AddMessageToLog(String message)
        {
            messageLog.AddLast(new Message(message));
            String result = "";
            // If it isn't the first one..
            for (int i = 0; i < messageLog.Count; i++)
            {
                if (i != 0) result += "\n";
                result += messageLog.ElementAt(i).GetComposedMessage();
            }
            messagesTextField.text = result;
        }
    }
}
