using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;

namespace PathfindingTest.Multiplayer.SocketConnection.InGame
{
    public class BuildingPacketProcessor
    {
        public void DataReceived(Packet p)
        {
            switch (p.GetHeader())
            {
                case Headers.KEEP_ALIVE:
                    {

                        break;
                    }
            }
        }
    }
}
