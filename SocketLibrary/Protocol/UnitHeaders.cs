using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketLibrary.Protocol
{
    public class UnitHeaders
    {
        /// <summary>
        /// Types to be used to request object IDs
        /// </summary>
        public const int TYPE_BOWMAN = 0, TYPE_SWORDMAN = 1, TYPE_ENGINEER = 2;


        /// <summary>
        /// Client synchronises a unit
        /// [Header] [Int32 serverID] [Int32 targetX] [Int32 targetY] [Int32 currentX] [Int32 currentY]
        /// </summary>
        public const byte GAME_UNIT_LOCATION = 0x30;

        /// <summary>
        /// Someone made a new unit. A unit location package will follow soon enough.
        /// [Header] [Int32 playerID] [Int32 serverID] [Int32 type]
        /// </summary>
        public const byte GAME_NEW_UNIT = 0x31;
    }
}
