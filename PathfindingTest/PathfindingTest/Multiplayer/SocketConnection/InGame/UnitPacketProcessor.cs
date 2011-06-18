using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using PathfindingTest.Units;
using PathfindingTest.Multiplayer.Data;
using Microsoft.Xna.Framework;

namespace PathfindingTest.Multiplayer.SocketConnection.InGame
{
    public class UnitPacketProcessor
    {
        public int locationMargin = 20;

        public void DataReceived(Packet p)
        {
            switch (p.GetHeader())
            {
                case UnitHeaders.GAME_UNIT_LOCATION:
                    {
                        int serverID = PacketUtil.DecodePacketInt(p, 0);
                        int targetX = PacketUtil.DecodePacketInt(p, 4);
                        int targetY = PacketUtil.DecodePacketInt(p, 8);
                        int currentX = PacketUtil.DecodePacketInt(p, 12);
                        int currentY = PacketUtil.DecodePacketInt(p, 16);

                        MultiplayerData data;
                        do {
                            data = MultiplayerDataManager.GetInstance().GetDataByServerID(serverID);
                            // Thread.
                        }
                        while (data == null);
                        Unit unit = ((UnitMultiplayerData)data).unit;

                        if (unit.waypoints.Count == 0 || unit.waypoints.Last.Value.X != targetX ||
                            unit.waypoints.Last.Value.Y != targetY)
                        {
                            Point target = new Point(targetX, targetY);
                            unit.multiplayerData.moveTarget = target;
                            unit.MoveToQueue(target);
                        }

                        if (Math.Abs(unit.x - currentX) > 20 || Math.Abs(unit.y - currentY) > 20)
                        {
                            // Uhoh .. we're too far apart :(
                            unit.x = currentX;
                            unit.y = currentY;
                        }


                        break;
                    }
                case UnitHeaders.GAME_NEW_UNIT:
                    {
                        int playerID = PacketUtil.DecodePacketInt(p, 0);
                        int serverID = PacketUtil.DecodePacketInt(p, 4);
                        int type = PacketUtil.DecodePacketInt(p, 8);

                        Unit unit = null;
                        switch (type)
                        {
                            case UnitHeaders.TYPE_BOWMAN:
                                {
                                    unit =
                                        Game1.GetInstance().GetPlayerByMultiplayerID(playerID).rangedStore.getUnit(Unit.Type.Ranged, 0, 0, 5);
                                    break;
                                }
                            case UnitHeaders.TYPE_ENGINEER:
                                {
                                    unit =
                                        Game1.GetInstance().GetPlayerByMultiplayerID(playerID).meleeStore.getUnit(Unit.Type.Engineer, 0, 0, 1);
                                    break;
                                }
                            case UnitHeaders.TYPE_SWORDMAN:
                                {
                                    unit =
                                        Game1.GetInstance().GetPlayerByMultiplayerID(playerID).meleeStore.getUnit(Unit.Type.Melee, 0, 0, 5);
                                    break;
                                }
                        }
                        Console.Out.WriteLine("New unit! : " + serverID);
                        unit.multiplayerData.serverID = serverID;

                        break;
                    }
            }
        }
    }
}
