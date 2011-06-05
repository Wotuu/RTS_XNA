using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfindingTest.Multiplayer
{
    public class MultiplayerGame
    {
        public int ID { get; set; }
        public String gamename { get; set; }
        public String username { get; set; }
        public String hostIP { get; set; }
        public String mapname { get; set; }
        public long creationDateMS { get; set; }

        public MultiplayerGame(int gameID, String gamename, String hostname, String hostIP, String mapname, long creationDateMS)
        {
            this.ID = gameID;
            this.gamename = gamename;
            this.username = hostname;
            this.hostIP = hostIP;
            this.mapname = mapname;
            this.creationDateMS = creationDateMS;
        }

        /// <summary>
        /// Creates this game in the database.
        /// </summary>
        /// <returns>True if the game was created, false otherwise.</returns>
        //public Boolean CreateGame()
        //{
           //  DatabaseManager.GetInstance().CreateGame(this);
        //}
    }
}
