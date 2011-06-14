using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using SocketLibrary.Users;
using XNAInterfaceComponents.AbstractComponents;
using PathfindingTest.UI.Menus;
using PathfindingTest.UI.Menus.Multiplayer;
using SocketLibrary.Multiplayer;
using XNAInterfaceComponents.ParentComponents;

namespace PathfindingTest.Multiplayer.SocketConnection
{
    public class GameLobbyPacketProcessor
    {

        public void DataReceived(Packet p)
        {
            ChatServerConnectionManager manager = ChatServerConnectionManager.GetInstance();
            switch (p.GetHeader())
            {
                case Headers.NEW_USER:
                    {
                        int userID = PacketUtil.DecodePacketInt(p, 0);
                        String username = PacketUtil.DecodePacketString(p, 4);
                        User user = new User(username);
                        user.id = userID;
                        user.channelID = manager.user.channelID;
                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();

                        
                        if (menu is GameLobby)
                        {
                            GameLobby lobby = ((GameLobby)menu);
                            lobby.UserJoined(user);
                            Console.Out.WriteLine(user + " has joined the game lobby!");
                        }

                        break;
                    }
                case Headers.USER_LEFT:
                    {
                        int userID = PacketUtil.DecodePacketInt(p, 0);
                        User user = UserManager.GetInstance().GetUserByID(userID);
                        if (user != null)
                        {
                            ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                            if (menu is GameLobby)
                            {
                                GameLobby lobby = ((GameLobby)menu);
                                lobby.UserLeft(user);
                                Console.Out.WriteLine(user + " User has left the game lobby!");
                            }
                        }

                        break;
                    }
                // Client received an ID for creating a game.
                case Headers.GAME_ID:
                    {
                        int gameID = PacketUtil.DecodePacketInt(p, 0);
                        Console.Out.WriteLine("Received game ID: " + gameID);

                        MultiplayerLobby lobby = ((MultiplayerLobby)MenuManager.GetInstance().GetCurrentlyDisplayedMenu());
                        String gameName = lobby.gameNameInput.textfield.text;

                        MenuManager.GetInstance().ShowMenu(MenuManager.Menu.GameLobby);
                        GameLobby gameLobby = (GameLobby)MenuManager.GetInstance().GetCurrentlyDisplayedMenu();

                        gameLobby.multiplayerGame = new MultiplayerGame(gameID,
                            gameName, "");
                        gameLobby.multiplayerGame.host = manager.user;
                        break;
                    }
                case Headers.SERVER_CREATE_GAME:
                    {
                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (menu is GameLobby)
                        {
                            // Confirmation that the game was created? idk
                        }
                        else if (menu is MultiplayerLobby)
                        {
                            Console.Out.WriteLine("Host is: " + manager.user);
                            MultiplayerGame game = new MultiplayerGame(
                                PacketUtil.DecodePacketInt(p, 0),
                                PacketUtil.DecodePacketString(p, 4),
                                "<No map selected yet>");
                            MultiplayerLobby lobby = (MultiplayerLobby)menu;
                            lobby.AddGame(game);
                        }
                        break;
                    }
                case Headers.SERVER_DESTROY_GAME:
                    {
                        int gameID = PacketUtil.DecodePacketInt(p, 0);

                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (menu is GameLobby)
                        {
                            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MultiplayerLobby);
                            XNAMessageDialog dialog =
                                XNAMessageDialog.CreateDialog("The host has disconnected.", XNAMessageDialog.DialogType.OK);
                            // When OK is pressed .. get back to the lobby.
                            /*dialog.button1.onClickListeners +=
                                delegate(XNAButton source)
                                {
                                    Packet newChannel = new Packet(Headers.CLIENT_CHANNEL);
                                    newChannel.AddInt(1);
                                    manager.connection.SendPacket(newChannel);
                                };*/

                        }
                        else if (menu is MultiplayerLobby)
                        {
                            MultiplayerLobby lobby = (MultiplayerLobby)menu;
                            lobby.RemoveGameByID(gameID);
                        }
                        break;
                    }
                case Headers.GAME_MAP_CHANGED:
                    {
                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (menu is MultiplayerLobby)
                        {
                            MultiplayerLobby lobby = (MultiplayerLobby)menu;
                            MultiplayerGame game = lobby.GetGameByID(PacketUtil.DecodePacketInt(p, 0));
                            if (game != null)
                            {
                                game.mapname = PacketUtil.DecodePacketString(p, 4);
                            }
                        }
                        break;
                    }
                case Headers.SERVER_REQUEST_JOIN:
                    {
                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (menu is GameLobby)
                        {
                            GameLobby lobby = (GameLobby)menu;
                            Packet newPacket = new Packet();
                            if (lobby.IsFull()) newPacket.SetHeader(Headers.CLIENT_GAME_FULL);
                            else newPacket.SetHeader(Headers.CLIENT_OK_JOIN);
                            newPacket.AddInt(PacketUtil.DecodePacketInt(p, 0));
                            newPacket.AddInt(PacketUtil.DecodePacketInt(p, 4));
                            manager.SendPacket(newPacket);
                        }
                        break;
                    }
                case Headers.CLIENT_OK_JOIN:
                    {
                        // Packet newPacket = new Packet(Headers.CLIENT_CHANNEL);
                        // newPacket.AddInt(PacketUtil.DecodePacketInt(p, 0));
                        MenuManager.GetInstance().ShowMenu(MenuManager.Menu.GameLobby);
                        // manager.SendPacket(newPacket);

                        break;
                    }
                case Headers.CLIENT_GAME_FULL:
                    {
                        XNAMessageDialog.CreateDialog("The game is full.", XNAMessageDialog.DialogType.OK);
                        break;
                    }
                default:
                    {
                        Console.Out.WriteLine("Received unknown request from the server (GameLobbyPacketProcessor, " + p.GetHeader() + ").");
                        break;
                    }
            }
        }
    }
}
