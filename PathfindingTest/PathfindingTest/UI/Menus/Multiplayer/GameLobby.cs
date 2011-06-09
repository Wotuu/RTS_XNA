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

namespace PathfindingTest.UI.Menus.Multiplayer
{
    public class GameLobby : XNAPanel
    {

        private XNATextField messagesTextField { get; set; }
        private XNATextField messageTextField { get; set; }
        public MultiplayerGame multiplayerGame { get; set; }

        public GameLobby()
            : base(null,
                new Rectangle(
                Game1.GetInstance().graphics.PreferredBackBufferWidth / 2 - 400,
                Game1.GetInstance().graphics.PreferredBackBufferHeight / 2 - 300,
                800, 600))
        {
            XNAPanel gameOptionsPanel = new XNAPanel(this, new Rectangle(5, 5, 500, 330));
            gameOptionsPanel.border = new Border(gameOptionsPanel, 1, Color.Blue);

            XNAPanel mapPreviewPanel = new XNAPanel(this, new Rectangle(510, 5, 790, 200));
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
            if (this.multiplayerGame.host != null)
            {
                // Destroy the game
                Packet destroyGame = new Packet(Headers.CLIENT_DESTROY_GAME);
                ChatServerConnectionManager.GetInstance().SendPacket(destroyGame);
            }

            // Change channel
            Packet channelPacket = new Packet(Headers.CLIENT_CHANNEL);
            channelPacket.AddInt(1);
            ChatServerConnectionManager.GetInstance().SendPacket(channelPacket);

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
    }
}
