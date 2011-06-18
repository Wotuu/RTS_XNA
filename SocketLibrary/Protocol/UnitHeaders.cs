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

        /// <summary>
        /// Client didn't receive data about a unit, when it did need it for processing.
        /// [Header] [Int32 requestingPlayerID] [Int32 serverID]
        /// </summary>
        public const byte GAME_REQUEST_UNIT_DATA = 0x32;

        /// <summary>
        /// Client that manages the data about this unit, will reply with the data
        /// and a movement update right after
        /// [Header] [Int32 requestingPlayerID] [Int32 owningPlayerID] [Int32 serverID] [Int32 type]
        /// </summary>
        public const byte GAME_SEND_UNIT_DATA = 0x33;
    }
}
