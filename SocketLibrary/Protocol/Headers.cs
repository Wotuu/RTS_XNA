﻿using System;
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
        // <byte header> <String username>
        public const byte CLIENT_USERNAME = 0x07;
        // Server sent the client a user ID
        // <byte header> <Int32 userID>
        public const byte CLIENT_USER_ID = 0x08;
        // Server sent the client the chat channel to be in
        // <byte header> <Int32 channelID>
        public const byte CLIENT_CHANNEL = 0x09;
        // Server sent the client a message that there is a new user
        // <byte header> <Int32 userID> <String username>
        public const byte NEW_USER = 0x0A;
        // Server sent the client a message that a user left the cannel.
        // <byte header> <Int32 userID> <String username>
        public const byte USER_LEFT = 0x0B;
        // Client wants to create creates a game
        // <byte header> <Int32 userID> <String gamename>
        public const byte CLIENT_CREATE_GAME = 0x0C;
        // Server notifies clients that there is a game created.
        // <byte header> <Int32 gameID> <String gamename>
        public const byte SERVER_CREATE_GAME = 0x0D;
        // Server sent the user a game ID
        public const byte GAME_ID = 0x0E;
        // Client notifies the server of a map change
        // <byte header> <Int32 gameID> <String mapname>
        public const byte GAME_MAP_CHANGED = 0x0F;
        // Client notifies the server that his game is destroyed.
        // <byte header> (only the current user can destroy the game)
        public const byte CLIENT_DESTROY_GAME = 0x10;
        // Server notifies clients that this game has been destroyed.
        // <byte header> <Int32 gameID>
        public const byte SERVER_DESTROY_GAME = 0x11;

        /// <summary>
        /// Client asks the server if he can join this game.
        /// <byte>header</byte>
        /// <Int32>gameID</Int32>
        /// <Int32>userID</Int32>
        /// </summary>
        public const byte CLIENT_REQUEST_JOIN = 0x12;
        // Server asks the host if a user can join this game
        // <byte header> <Int32>gameID</Int32> <Int32 userID>
        public const byte SERVER_REQUEST_JOIN = 0x13;
        
        // Host replies that it is OK to join the server.
        // Also, is used by the server to send the requesting
        // user that it is OK to join.
        // <byte header> <Int32>gameID</Int32> <Int32 userID>
        public const byte CLIENT_OK_JOIN = 0x14;
        // Host replies that the game is full
        // Also, is used by the server to send the requesting
        // user that it is not OK to join.
        // <byte header> <Int32>gameID</Int32> <Int32 userID>
        public const byte CLIENT_GAME_FULL = 0x15;

        /// <summary>
        /// Client notifies the server that he's left the game.
        /// </summary>
        public const byte CLIENT_LEFT_GAME = 0x16;



        // 16 - 255 Yours to take
    }
}
