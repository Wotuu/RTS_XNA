using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PathfindingTest.QuadTree;
using PathfindingTest.Players;
using XNAInputHandler.MouseInput;
using PathfindingTest.UI.Menus;

namespace PathfindingTest.State
{
    public class StateManager
    {
        private State _gameState { get; set; }
        public State gameState
        {
            get
            {
                return _gameState;
            }
            set
            {
                if (value == State.MainMenu)
                {
                    MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MainMenu);
                }
                else if (value == State.GameInit)
                {
                    Game1 game = Game1.GetInstance();
                    (game.quadTree = new QuadRoot(new Rectangle(0, 0,
                        game.graphics.PreferredBackBufferWidth, game.graphics.PreferredBackBufferHeight)
                        )).CreateTree(5);

                    game.players = new LinkedList<Player>();

                    Alliance redAlliance = new Alliance();
                    Player humanPlayer = new Player(redAlliance, Color.Red);
                    game.players.AddLast((Game1.CURRENT_PLAYER = humanPlayer));
                    humanPlayer.SpawnStartUnits(new Point((int)Game1.GetInstance().graphics.PreferredBackBufferWidth / 2,
                        (int)Game1.GetInstance().graphics.PreferredBackBufferWidth / 2));




                    Alliance greenAlliance = new Alliance();
                    Player aiPlayer = new Player(greenAlliance, Color.Green);
                    game.players.AddLast(aiPlayer);
                    aiPlayer.SpawnStartUnits(new Point((int)Game1.GetInstance().graphics.PreferredBackBufferWidth / 2, 200));

                    //SaveManager.GetInstance().SaveNodes("C:\\Users\\Wouter\\Desktop\\test.xml");
                    MouseManager.GetInstance().mouseClickedListeners += ((MouseClickListener)game).OnMouseClick;
                    MouseManager.GetInstance().mouseReleasedListeners += ((MouseClickListener)game).OnMouseRelease;
                    MouseManager.GetInstance().mouseMotionListeners += ((MouseMotionListener)game).OnMouseMotion;
                    MouseManager.GetInstance().mouseDragListeners += ((MouseMotionListener)game).OnMouseDrag;
                }
                else if (value == State.GameRunning)
                {

                }
                else if (value == State.GamePaused)
                {

                }
                else if (value == State.GameShutdown)
                {

                }

                Console.Out.WriteLine("State is now " + value.ToString());
                _gameState = value;
            }
        }
        private static StateManager instance;
        private StateManager() { }

        public enum State
        {
            MainMenu,
            GameInit,
            GameRunning,
            GamePaused,
            GameShutdown
        }



        public static StateManager GetInstance()
        {
            if (instance == null) instance = new StateManager();
            return instance;
        }


    }
}
