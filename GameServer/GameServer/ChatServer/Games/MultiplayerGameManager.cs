using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary.Multiplayer;
using GameServer.ChatServer.Channels;

namespace GameServer.ChatServer.Games
{
    public class MultiplayerGameManager
    {
        private static MultiplayerGameManager instance;
        public LinkedList<MultiplayerGame> games = new LinkedList<MultiplayerGame>();

        private MultiplayerGameManager() { }

        public static MultiplayerGameManager GetInstance()
        {
            if (instance == null) instance = new MultiplayerGameManager();
            return instance;
        }

        /// <summary>
        /// Requests a new game ID.
        /// </summary>
        /// <returns>The new game ID that you should use.</returns>
        public int RequestGameID()
        {
            return ChannelManager.GetInstance().RequestChannelID();
        }
    }
}
