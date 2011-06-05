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

namespace PathfindingTest.UI.Menus.Multiplayer
{
    public class MultiplayerLobby : XNAPanel
    {
        private XNATextField messagesTextField { get; set; }
        private XNATextField messageTextField { get; set; }
        private XNATextField usersField { get; set; }


        public MultiplayerLobby()
            : base(null, new Rectangle(
                Game1.GetInstance().graphics.PreferredBackBufferWidth / 2 - 400,
                Game1.GetInstance().graphics.PreferredBackBufferHeight / 2 - 300,
                800, 600))
        {
            XNAPanel gamesPanel = new XNAPanel(this, new Rectangle(5, 5, 590, 330));
            gamesPanel.border = new Border(gamesPanel, 1, Color.Blue);

            XNAPanel usersPanel = new XNAPanel(this, new Rectangle(600, 5, 195, 330));
            usersPanel.border = new Border(usersPanel, 1, Color.Blue);

            usersField = new XNATextField(usersPanel, new Rectangle(5, 5, 185, 320), Int32.MaxValue);
            usersField.font = MenuManager.SMALL_TEXTFIELD_FONT;
            usersField.isEditable = false;




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

            XNAButton disconnectButton = new XNAButton(this,
                new Rectangle(this.bounds.Width - 105, this.bounds.Height - 45, 100, 40), "Disconnect");
            disconnectButton.onClickListeners += DisconnectBtnClicked;
        }

        /// <summary>
        /// The user wants to disconnect.
        /// </summary>
        /// <param name="source"></param>
        public void DisconnectBtnClicked(XNAButton source)
        {
            ChatServerConnectionManager.GetInstance().DisconnectFromServer();
            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MultiplayerLogin);
        }

        /// <summary>
        /// Adds a message to the log.
        /// </summary>
        /// <param name="message">The message to add</param>
        public void AddMessageToLog(String message)
        {
            String result = "";
            // If it isn't the first one..
            if (messagesTextField.text.Length != 0)
            {
                if (!message.StartsWith("\n"))
                {
                    result += "\n";
                }
            }
            messagesTextField.text += result + "[" + System.DateTime.UtcNow.ToLongTimeString() + "] " + message;
        }

        /// <summary>
        /// Adds a user to the user log.
        /// </summary>
        /// <param name="user">The user to add</param>
        public void AddUser(User user)
        {
            String result = "(" + user.id + ") " + user.username;
            // If it isn't the first one ..
            if (usersField.text.Length != 0)
            {
                if (!user.username.StartsWith("\n"))
                {
                    result = "\n" + "(" + user.id + ") " + user.username;
                }
            }
            usersField.text += result;
        }

        /// <summary>
        /// Removes a user from the list.
        /// </summary>
        /// <param name="user">The user to remove.</param>
        public void RemoveUser(User user)
        {
            usersField.text.Replace("\n" + "(" + user.id + ") " + user.username, "");
        }

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
                packet.AddInt(chat.user.channel);
                packet.AddString(message);
                chat.SendPacket(packet);
            }
        }
    }
}
