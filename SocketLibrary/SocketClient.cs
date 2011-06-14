using System;
using System.Net.Sockets;
using SocketLibrary.Packets;
using System.Collections;
using SocketLibrary.Protocol;
using System.Collections.Generic;


public delegate void OnDisconnectListeners();
public delegate void OnPacketSend(Packet p);
namespace SocketLibrary
{
    public class SocketClient
    {
        public Socket Sock;
        public string SocketName;
        private byte[] buff = new byte[1024];
        public bool Receiving = true;
        private readonly object SyncRecv;
        private readonly object SyncSend;
        public OnDisconnectListeners onDisconnectListeners { get; set; }
        public LinkedList<LogMessage> messageLog = new LinkedList<LogMessage>();

        public PacketProcessor packetProcessor { get; set; }
        public OnPacketSend onPacketSendListeners { get; set; }

        public SocketClient(Socket _sock, string _socketname)
        {
            SocketName = _socketname;
            Sock = _sock;
            Sock.NoDelay = true;
            SyncRecv = new object();
            SyncSend = new object();
        }

        /// <summary>
        /// Enables the socket to accept packets
        /// </summary>
        public void Enable()
        {
            if (this.packetProcessor == null) packetProcessor = new PacketProcessor();
            packetProcessor.onProcessPacket += this.OnProcessPacket;
            packetProcessor.StartProcessing();

            lock (SyncRecv)
            {
                Console.Out.WriteLine("Starting receiving packets..");
                while (Receiving)
                {
                    if (Sock.Connected)
                    {
                        int size = 0;
                        try
                        {
                            size = Sock.Receive(buff);
                        }
                        catch (Exception e)
                        {
                            this.Disable();
                            break;
                        }
                        if (size == 0)
                        {
                            Disable();
                            Show("Will not accept 'byte[i <= 0]'");
                            Receiving = false;
                            break;
                        }
                        if (size < 1000)
                        {
                            byte[] data = new byte[size];
                            Array.Copy(buff, data, size);
                            //TODO: Should go to a SocketProcessor here.


                            byte[] headerlessData = new byte[data.Length - 1];
                            for (int i = 1; i < data.Length; i++)
                            {
                                headerlessData[i - 1] = data[i];
                            }
                            Packet p = new Packet(data[0], headerlessData);
                            this.packetProcessor.QueuePacket(p);
                        }
                        else
                        {
                            Disable();
                            Show("Cannot accept 'bytes[i > 1000]'");
                            Receiving = false;
                            break;
                        }
                    }
                }
                if (onDisconnectListeners != null) onDisconnectListeners();
                Console.Out.WriteLine("Ending receiving packets..");
            }
            packetProcessor.StopProcessing();
        }

        /// <summary>
        /// Packet processor has determined that the packet needed processing.
        /// </summary>
        /// <param name="p">The packet that needed processing</param>
        public void OnProcessPacket(Packet p)
        {
            Log(p, true);
        }

        /// <summary>
        /// Sends a packet to the Socket we're connected with.
        /// </summary>
        /// <param name="packet">The packet to send</param>
        public void SendPacket(Packet packet)
        {
            lock (SyncSend)
            {
                if (Sock.Connected)
                {
                    try
                    {
                        byte[] data = packet.GetFullData();
                        Sock.Send(data, data.Length, SocketFlags.None);
                        Log(packet, false);
                        if (onPacketSendListeners != null) onPacketSendListeners(packet);
                    }
                    catch (Exception ex)
                    {
                        Show("Unable to sent a packet.");
                        Show(ex);
                    }
                }
                else
                    Show("Fail. You shouldn't be able to make it send a packet, without having a connection.");
            }
        }


        /// <summary>
        /// Gets the IP address of this client in string representation
        /// </summary>
        /// <returns>The String</returns>
        public String GetRemoteHostIP()
        {
            String result = "";
            try
            {
                result = this.Sock.RemoteEndPoint.ToString();
            }
            catch (ObjectDisposedException e)
            {
                result = "SOCKET DISPOSED";
            }
            return result;
        }

        /// <summary>
        /// Logs the data to the logger.
        /// </summary>
        /// <param name="data"></param>
        public void Log(Packet p, Boolean isReceived)
        {
            String currTime = System.DateTime.Now.ToLongTimeString()+"," + System.DateTime.Now.Millisecond + " ";
            switch (p.GetHeader())
            {
                case Headers.HANDSHAKE_1:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "HANDSHAKE_1 Received handshake request (1)", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "HANDSHAKE_1 Sent handshake request (1)", isReceived));
                        break;
                    }
                case Headers.HANDSHAKE_2:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "HANDSHAKE_2 Received handshake acknowledge (2)", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "HANDSHAKE_2 Sent handshake acknowledge (2)", isReceived));
                        break;
                    }
                case Headers.HANDSHAKE_3:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "HANDSHAKE_3 Received handshake confirmation (3)", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "HANDSHAKE_3 Sent handshake confirmation (3)", isReceived));
                        break;
                    }
                case Headers.KEEP_ALIVE:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "KEEP_ALIVE User is still connected (still alive)", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "KEEP_ALIVE Asking client if he's still alive ", isReceived));
                        break;
                    }
                case Headers.CHAT_MESSAGE:
                    {
                        if (isReceived)
                            this.messageLog.AddLast(new LogMessage(currTime + "CHAT_MESSAGE Received sent a chat message in channel " + PacketUtil.DecodePacketInt(p, 0) +
                                ": " + PacketUtil.DecodePacketString(p, 4), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CHAT_MESSAGE Sent chat message to all in channel " + PacketUtil.DecodePacketInt(p, 0) +
                                ": " + PacketUtil.DecodePacketString(p, 4), isReceived));
                        break;
                    }
                case Headers.CLIENT_DISCONNECT:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_DISCONNECT Client disconnected", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_DISCONNECT Disconnected this client", isReceived));
                        break;
                    }
                case Headers.SERVER_DISCONNECT:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "SERVER_DISCONNECT Server disconnected", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "SERVER_DISCONNECT Disconnected from server", isReceived));
                        break;
                    }
                case Headers.CLIENT_USERNAME:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_USERNAME Requested a username: " +
                            PacketUtil.DecodePacketString(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_USERNAME Confirming requested username: " + PacketUtil.DecodePacketString(p, 0), isReceived));
                        break;
                    }
                case Headers.CLIENT_USER_ID:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_USER_ID Requested a userid: " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_USER_ID Confirming requested userid: " + PacketUtil.DecodePacketInt(p, 0), isReceived));
                        break;
                    }
                case Headers.CLIENT_CHANNEL:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_CHANNEL Received change channel request to " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_CHANNEL Sent change channel to " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        break;
                    }
                case Headers.NEW_USER:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "NEW_USER Received new user: " +
                            PacketUtil.DecodePacketString(p, 4), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "NEW_USER Sent new user: " +
                            PacketUtil.DecodePacketString(p, 4), isReceived));
                        break;
                    }
                case Headers.USER_LEFT:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "USER_LEFT Received user has left: " +
                            PacketUtil.DecodePacketString(p, 4), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "USER_LEFT Sent user has left: " +
                            PacketUtil.DecodePacketString(p, 4), isReceived));
                        break;
                    }
                case Headers.CLIENT_CREATE_GAME:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_CREATE_GAME Received game creation request: userid = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", gamename = " +
                            PacketUtil.DecodePacketString(p, 4), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_CREATE_GAME Sent game creation request: userid = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", gamename = " +
                            PacketUtil.DecodePacketString(p, 4), isReceived));
                        break;
                    }
                case Headers.SERVER_CREATE_GAME:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "SERVER_CREATE_GAME Received server game creation request: " +
                            "gameid = " + PacketUtil.DecodePacketInt(p, 0) +
                            ", gamename = " + PacketUtil.DecodePacketString(p, 4), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "SERVER_CREATE_GAME Sent server game creation request: " +
                            "gameid = " + PacketUtil.DecodePacketInt(p, 0) +
                            ", gamename = " + PacketUtil.DecodePacketString(p, 4), isReceived));
                        break;
                    }
                case Headers.GAME_ID:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_ID Received a game ID: " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_ID Sent a game ID to use: " +
                           PacketUtil.DecodePacketInt(p, 0), isReceived));
                        break;
                    }
                case Headers.GAME_MAP_CHANGED:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_MAP_CHANGED Received game map of game id = " +
                            PacketUtil.DecodePacketInt(p, 0) + " has changed to -> " +
                            PacketUtil.DecodePacketString(p, 4), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_MAP_CHANGED Sent that game map of game id = " +
                           PacketUtil.DecodePacketInt(p, 0) + " has changed to -> " +
                           PacketUtil.DecodePacketString(p, 4), isReceived));
                        break;
                    }
                case Headers.CLIENT_DESTROY_GAME:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_DESTROY_GAME Received destroy game request by client!", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_DESTROY_GAME Sent destroy game request.", isReceived));
                        break;
                    }
                case Headers.SERVER_DESTROY_GAME:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "SERVER_DESTROY_GAME Received destroy game request of game " +
                           PacketUtil.DecodePacketInt(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "SERVER_DESTROY_GAME Sent destroy game request of game " +
                           PacketUtil.DecodePacketInt(p, 0) + " to all clients.", isReceived));
                        break;
                    }
                case Headers.CLIENT_REQUEST_JOIN:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_REQUEST_JOIN Received request to join game " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_REQUEST_JOIN Sent request to join game " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        break;
                    }
                case Headers.SERVER_REQUEST_JOIN:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "SERVER_REQUEST_JOIN Received request if user " +
                            PacketUtil.DecodePacketInt(p, 4) + " can join my game " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "SERVER_REQUEST_JOIN Sent request if user " +
                            PacketUtil.DecodePacketInt(p, 4) + " can join my game " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        break;
                    }
                case Headers.CLIENT_OK_JOIN:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_OK_JOIN Received OK to join game. ", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_OK_JOIN Sent OK to join game. ", isReceived));
                        break;
                    }
                case Headers.CLIENT_GAME_FULL:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_GAME_FULL Received game full message. ", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_GAME_FULL Sent game full message.", isReceived));
                        break;
                    }
                case Headers.CLIENT_LEFT_GAME:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_LEFT_GAME Received client has left game message. ", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_LEFT_GAME Sent client has left message.", isReceived));
                        break;
                    }
                /*
                 * 
                    if (isReceived) this.messageLog.AddLast(new LogMessage( currTime + " ",isReceived ));
                    else this.messageLog.AddLast(new LogMessage( currTime + " ",isReceived ));
                 *
                 */
                default:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + " Received an unknown request (" + p.GetHeader() + ") "
                            + "(or have you forgotten to add the header to the log?)", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + " Sent unknown request (" + p.GetHeader() + ") "
                            + "(or have you forgotten to add the header to the log?)", isReceived));
                        break;
                    }
            }
        }

        /// <summary>
        /// Disables the socket client.
        /// </summary>
        public void Disable()
        {
            Receiving = false;
            Sock.Close();
        }

        public void Show(string Text)
        {
            Console.Write(SocketName + ": " + Text);
        }

        public void Show(Exception Text)
        {
            Console.Write(SocketName + " ERROR: " + Text);
        }



        public class LogMessage
        {
            public String message { get; set; }
            public Boolean received { get; set; }

            public LogMessage(String message, Boolean received)
            {
                this.message = message;
                this.received = received;
            }
        }
    }
}
