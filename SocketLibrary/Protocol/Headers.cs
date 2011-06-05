using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketLibrary.Protocol
{
    public class Headers
    {
        // 0 - 15 Default reserved ranges

        // Client requests a connection
        public const byte HANDSHAKE_1 = 0x00;
        // Server replies
        public const byte HANDSHAKE_2 = 0x01;
        // Client confirms
        public const byte HANDSHAKE_3 = 0x02;
        // Keep the connection open
        public const byte KEEP_ALIVE = 0x03;
        // Chat message header, packet is in the following format:
        // <byte header> <Int32 channel> <String message>
        public const byte CHAT_MESSAGE = 0x04;
        // Client terminated the connection
        public const byte CLIENT_DISCONNECT = 0x05;
        // Server terminated the connection
        public const byte SERVER_DISCONNECT = 0x06;
        // Client sent the server its username
        public const byte CLIENT_USERNAME = 0x07;
        // Server sent the client a user ID
        public const byte CLIENT_USER_ID = 0x08;
        // Server sent the client the chat channel to be in
        public const byte CLIENT_CHANNEL = 0x09;
        // Server sent the client a message that there is a new user
        // <byte header> <Int32 userID> <String username>
        public const byte NEW_USER = 0x0A;
        // Server sent the client a message that a user left the cannel.
        // <byte header> <Int32 userID> <String username>
        public const byte USER_LEFT = 0x0B;



        // 16 - 255 Yours to take
    }
}
