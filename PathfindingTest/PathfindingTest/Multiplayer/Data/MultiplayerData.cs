using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Multiplayer.SocketConnection.InGame;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;

namespace PathfindingTest.Multiplayer.Data
{
    public abstract class MultiplayerData
    {
        public Boolean isLocal { get; set; }
        public int localID { get; set; }
        public int serverID { get; set; }
        /// <summary>
        /// Variable for checking if the unit should exist on the server.
        /// When an object is queued, and this is false, it will pulse the server first of it's existance.
        /// </summary>
        public Boolean isCreated { get; set; }
        public double lastPulse { get; set; }

        public MultiplayerData(Boolean isLocal)
        {
            this.isLocal = isLocal;
            this.serverID = -1;
            this.localID = (Game1.GetInstance().objectsCreated++);
            MultiplayerDataManager.GetInstance().AddData(this);
        }

        /// <summary>
        /// This object will request a server id, and notify the rest of the players of its
        /// creation.
        /// </summary>
        /// <param name="type">The unit type as defined in UnitHeaders.cs</param>
        public void RequestServerID(int type)
        {
            Packet p = new Packet(Headers.GAME_REQUEST_OBJECT_ID);
            p.AddInt(localID);
            p.AddInt(type);

            GameServerConnectionManager.GetInstance().SendPacket(p);
        }

        public abstract int GetUnitType();
    }
}
