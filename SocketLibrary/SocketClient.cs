using System;
using System.Net.Sockets;
using SocketLibrary.Packets;
using System.Collections;
using SocketLibrary.Protocol;


public delegate void OnDisconnectListeners();
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
        public ArrayList messageLog = new ArrayList();

        public PacketProcessor packetProcessor { get; set; }

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
            if( this.packetProcessor == null ) packetProcessor = new PacketProcessor();
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
                        Console.Out.WriteLine("Sending packet with header " + packet.GetHeader());
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
            String currTime = System.DateTime.Now.ToLongTimeString();
            switch (p.GetHeader())
            {
                case Headers.HANDSHAKE_1:
                    {
                        if (isReceived) this.messageLog.Add(currTime + " Received handshake request (1)");
                        else this.messageLog.Add(currTime + " Sent handshake request (1)");
                        break;
                    }
                case Headers.HANDSHAKE_2:
                    {
                        if (isReceived) this.messageLog.Add(currTime + " Received handshake acknowledge (2)");
                        else this.messageLog.Add(currTime + " Sent handshake acknowledge (2)");
                        break;
                    }
                case Headers.HANDSHAKE_3:
                    {
                        if (isReceived) this.messageLog.Add(currTime + " Received handshake confirmation (3)");
                        else this.messageLog.Add(currTime + " Sent handshake confirmation (3)");
                        break;
                    }
                case Headers.KEEP_ALIVE:
                    {
                        if (isReceived) this.messageLog.Add(currTime + " User is still connected (still alive)");
                        else this.messageLog.Add(currTime + " Asking client if he's still alive ");
                        break;
                    }
                case Headers.CHAT_MESSAGE:
                    {
                        if (isReceived)
                            this.messageLog.Add(currTime + " Received sent a chat message in channel " + PacketUtil.DecodePacketInt(p, 0) +
                                ": " + PacketUtil.DecodePacketString(p, 4));
                        else this.messageLog.Add(currTime + " Sent chat message to all in channel " + PacketUtil.DecodePacketInt(p, 0) +
                                ": " + PacketUtil.DecodePacketString(p, 4));
                        break;
                    }
                case Headers.CLIENT_DISCONNECT:
                    {
                        if (isReceived) this.messageLog.Add(currTime + " Client disconnected");
                        else this.messageLog.Add(currTime + " Disconnected this client");
                        break;
                    }
                case Headers.SERVER_DISCONNECT:
                    {
                        if (isReceived) this.messageLog.Add(currTime + " Server disconnected");
                        else this.messageLog.Add(currTime + " Disconnected from server");
                        break;
                    }
                case Headers.CLIENT_USERNAME:
                    {
                        if (isReceived) this.messageLog.Add(currTime + " Requested a username: " +
                            PacketUtil.DecodePacketString(p, 0));
                        else this.messageLog.Add(currTime + " Confirming requested username: " + PacketUtil.DecodePacketString(p, 0));
                        break;
                    }
                case Headers.CLIENT_USER_ID:
                    {
                        if (isReceived) this.messageLog.Add(currTime + " Requested a userid: " +
                            PacketUtil.DecodePacketInt(p, 0));
                        else this.messageLog.Add(currTime + " Confirming requested userid: " + PacketUtil.DecodePacketInt(p, 0));
                        break;
                    }
                case Headers.CLIENT_CHANNEL:
                    {
                        if (isReceived) this.messageLog.Add(currTime + " Received change channel request to " +
                            PacketUtil.DecodePacketInt(p, 0));
                        else this.messageLog.Add(currTime + " Sent change channel to " +
                            PacketUtil.DecodePacketInt(p, 0));
                        break;
                    }
                case Headers.NEW_USER:
                    {
                        if (isReceived) this.messageLog.Add(currTime + " Received new user: " +
                            PacketUtil.DecodePacketString(p, 4));
                        else this.messageLog.Add(currTime + " Sent new user: " +
                            PacketUtil.DecodePacketString(p, 4));
                        break;
                    }
                case Headers.USER_LEFT:
                    {
                        if (isReceived) this.messageLog.Add(currTime + " Received user has left: " +
                            PacketUtil.DecodePacketString(p, 4));
                        else this.messageLog.Add(currTime + " Sent user has left: " +
                            PacketUtil.DecodePacketString(p, 4));
                        break;
                    }
                case Headers.CLIENT_CREATE_GAME:
                    {
                        if (isReceived) this.messageLog.Add(currTime + " Received game creation request: userid = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", gamename = " + 
                            PacketUtil.DecodePacketString(p, 4));
                        else this.messageLog.Add(currTime + " Sent game creation request: userid = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", gamename = " + 
                            PacketUtil.DecodePacketString(p, 4));
                        break;
                    }
                case Headers.SERVER_CREATE_GAME:
                    {
                        if (isReceived) this.messageLog.Add(currTime + " Received server game creation request: " +
                            "gameid = " + PacketUtil.DecodePacketInt(p, 0) + 
                            "userid = " + PacketUtil.DecodePacketInt(p, 4) + 
                            ", gamename = " + PacketUtil.DecodePacketString(p, 8));
                        else this.messageLog.Add(currTime + " Sent server game creation request: " +
                            "gameid = " + PacketUtil.DecodePacketInt(p, 0) +
                            "userid = " + PacketUtil.DecodePacketInt(p, 4) +
                            ", gamename = " + PacketUtil.DecodePacketString(p, 8));
                        break;
                    }
                case Headers.GAME_ID:
                    {
                        if (isReceived) this.messageLog.Add(currTime + " Received a game ID: " + 
                            PacketUtil.DecodePacketInt(p, 0));
                        else this.messageLog.Add(currTime + " Sent a game ID to use: " +
                           PacketUtil.DecodePacketInt(p, 0));
                        break;
                    }
                case Headers.GAME_MAP_CHANGED:
                    {
                        if (isReceived) this.messageLog.Add(currTime + " Received game map of game id = " +
                            PacketUtil.DecodePacketInt(p, 0) + " has changed to -> " +
                            PacketUtil.DecodePacketString(p, 4));
                        else this.messageLog.Add(currTime + " Sent that game map of game id = " +
                           PacketUtil.DecodePacketInt(p, 0) + " has changed to -> " +
                           PacketUtil.DecodePacketString(p, 4));
                        break;
                    }

                    /*
                     * 
                        if (isReceived) this.messageLog.Add(currTime + " ");
                        else this.messageLog.Add(currTime + " ");
                     *
                     */ 
                default:
                    {
                        if (isReceived) this.messageLog.Add(currTime + " Received an unknown request (" + p.GetHeader() + ") "
                            + "(or have you forgotten to add the header to the log?)");
                        else this.messageLog.Add(currTime + " Sent unknown request (" + p.GetHeader() + ") "
                            + "(or have you forgotten to add the header to the log?)");
                        break;
                    }
            }
        }

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
    }
}
