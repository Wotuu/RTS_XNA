using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using PathfindingTest;
using System.Diagnostics;
using PathfindingTest.Buildings;
using PathfindingTest.Players;
using PathfindingTest.Units;
using PathfindingTest.Pathfinding;
using XNAInputHandler.MouseInput;

namespace PathfindingTest.UI
{
    public class HUD : MouseClickListener
    {

        Player player;
        Color color;

        Texture2D hudTex;
        Texture2D hudResourcesTex;
        Texture2D hudBarracksTex;
        Texture2D hudFactoryTex;
        Texture2D hudFortressTex;

        Texture2D hudItemDetails;

        SpriteFont sf;

        public Boolean loadForEngineer { get; set; }
        public Boolean draw { get; set; }

        Boolean drawResourcesText = false;
        Boolean drawBarracksText = false;
        Boolean drawFactoryText = false;
        Boolean drawFortressText = false;

        /// <summary>
        /// HUD Constructor.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="cm"></param>
        /// <param name="c"></param>
        public HUD(Player p, Color c)
        {
            this.player = p;
            this.color = c;

            hudTex = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUD");
            hudResourcesTex = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDResources");
            hudBarracksTex = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDBarracks");
            hudFactoryTex = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDFactory");
            hudFortressTex = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDFortress");

            hudItemDetails = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDItemDetails");

            sf = Game1.GetInstance().Content.Load<SpriteFont>("Fonts/SpriteFont1");

            loadForEngineer = false;
            draw = false;

            MouseManager.GetInstance().mouseClickedListeners += ((MouseClickListener)this).OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners += ((MouseClickListener)this).OnMouseRelease;
        }

        /// <summary>
        /// Standard Update function.
        /// </summary>
        /// <param name="ks"></param>
        /// <param name="ms"></param>
        public void Update(KeyboardState ks, MouseState ms)
        {
            CheckDraw();
            CountUnits();

            Rectangle mr = new Rectangle(ms.X, ms.Y, 1, 1);
            drawResourcesText = this.DefineResourcesRectangle().Contains(mr);
            drawBarracksText = this.DefineBarracksRectangle().Contains(mr);
            drawFactoryText = this.DefineFactoryRectangle().Contains(mr);
            drawFortressText = this.DefineFortressRectangle().Contains(mr);
        }

        /// <summary>
        /// Checks if the HUD should be drawn.
        /// If an Engineer is selected, load contents for Engineer.
        /// </summary>
        /// <param name="sb"></param>
        internal void Draw(SpriteBatch sb)
        {
            if (draw)
            {
                sb.Draw(hudTex, new Rectangle(0, 652, 1024, 116), color);
            }
            else return;

            if (loadForEngineer)
            {
                sb.Draw(hudResourcesTex, new Rectangle(278, 688, 28, 28), color);
                sb.Draw(hudBarracksTex, new Rectangle(316, 688, 28, 28), color);
                sb.Draw(hudFactoryTex, new Rectangle(354, 688, 28, 28), color);
                sb.Draw(hudFortressTex, new Rectangle(392, 688, 28, 28), color);
            }

            if (drawResourcesText)
            {
                Rectangle rect = DefineDetailsRectangle();
                sb.Draw(hudItemDetails, rect, color);
                sb.DrawString(sf, "Resources", new Vector2(rect.X + 5, rect.Y + 5), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0f);
            }

            if (drawBarracksText)
            {
                Rectangle rect = DefineDetailsRectangle();
                sb.Draw(hudItemDetails, rect, color);
                sb.DrawString(sf, "Barracks", new Vector2(rect.X + 5, rect.Y + 5), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0f);
            }

            if (drawFactoryText)
            {
                Rectangle rect = DefineDetailsRectangle();
                sb.Draw(hudItemDetails, rect, color);
                sb.DrawString(sf, "Factory", new Vector2(rect.X + 5, rect.Y + 5), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0f);
            }

            if (drawFortressText)
            {
                Rectangle rect = DefineDetailsRectangle();
                sb.Draw(hudItemDetails, rect, color);
                sb.DrawString(sf, "Fortress", new Vector2(rect.X + 5, rect.Y + 5), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0f);
            }
        }

        void MouseClickListener.OnMouseClick(MouseEvent me)
        {
            if (me.button == MouseEvent.MOUSE_BUTTON_1 &&
                player.currentSelection != null &&
                //player.currentSelection.units.Count == 1 &&
                //player.currentSelection.units.ElementAt(0).type == Unit.UnitType.Engineer &&
                loadForEngineer)
            {
                if (drawResourcesText || drawBarracksText || drawFactoryText || drawFortressText)
                {
                    Console.Out.WriteLine("Previewing a building!");
                    player.RemovePreviewBuildings();
                    Building b;

                    if (drawResourcesText)
                    {
                        b = new ResourceGather(this.player, this.color);
                    }
                    else if (drawBarracksText)
                    {
                        b = new Barracks(this.player, this.color);
                    }
                    else if (drawFactoryText)
                    {
                        b = new Factory(this.player, this.color);
                    }
                    else if (drawFortressText)
                    {
                        b = new Fortress(this.player, this.color);
                    }
                    Console.Out.WriteLine("Creating a building");
                    // player.buildings.AddLast(b);
                }

                foreach (Building b in player.buildings)
                {
                    if (b.state == Building.BuildState.Preview &&
                        !this.DefineRectangle().Contains(new Rectangle(me.location.X, me.location.Y, 1, 1)) &&
                        Game1.GetInstance().collision.CanPlace(b.DefineRectangle()))
                    {
                        Engineer temp = null;

                        foreach (Unit u in player.currentSelection.units)
                        {
                            if (u.type == Unit.UnitType.Engineer)
                            {
                                u.MoveToNow(new Point(me.location.X, me.location.Y));
                                // Get the last point of the pathfinding result
                                Point lastPoint = u.waypoints.ElementAt(u.waypoints.Count - 1);
                                // Remove that point
                                u.waypoints.RemoveLast();
                                // Add a point that is on the circle near the building, not inside the building!
                                Point targetPoint = new Point(0, 0);
                                if (u.waypoints.Count == 0) targetPoint = new Point((int)u.x, (int)u.y);
                                else targetPoint = u.waypoints.ElementAt(u.waypoints.Count - 1);
                                // Move to the point around the circle of the building, but increase the radius a bit
                                // so we're not standing on the exact top of the building
                                u.waypoints.AddLast(
                                    Util.GetPointOnCircle(lastPoint, b.GetCircleRadius() + u.texture.Width / 2,
                                    Util.GetHypoteneuseAngleDegrees(lastPoint, targetPoint)));

                                // Set the Engineer to link with the Building
                                // so construction won't start without an Engineer
                                // Since only one Engineer is needed, break aftwards
                                if (temp == null)
                                {
                                    temp = (Engineer) u;
                                    break;
                                }
                            }
                        }

                        b.PlaceBuilding(temp);
                    }
                }
            }
        }

        void MouseClickListener.OnMouseRelease(MouseEvent me)
        {
        }

        /// <summary>
        /// Count the different unit types selected.
        /// Determines how the HUD should be loaded.
        /// </summary>
        public void CountUnits()
        {
            int engineerCounter = 0;

            if (player.currentSelection != null)
            {
                foreach (Unit u in player.currentSelection.units)
                {
                    if (u.type == Unit.UnitType.Engineer)
                    {
                        engineerCounter++;
                    }
                }
            }

            if (engineerCounter > 0)
            {
                loadForEngineer = true;
            }
            else
            {
                loadForEngineer = false;
            }
        }

        public void CheckDraw()
        {
            if (player.currentSelection == null)
            {
                draw = false;
                return;
            }

            if (player.currentSelection.units.Count > 0)
            {
                draw = true;
            }
            else
            {
                draw = false;
            }
        }

        /// <summary>
        /// Whether the mouse is over a building preview text or not.
        /// </summary>
        /// <returns>Yes or no.</returns>
        public Boolean IsMouseOverBuilding()
        {
            // If either is true, return true
            return drawResourcesText | drawFortressText | drawFactoryText | drawBarracksText;
        }

        public Rectangle DefineRectangle()
        {
            return new Rectangle(195, 652, 634, 116);
        }

        public Rectangle DefineDetailsRectangle()
        {
            return new Rectangle(0, 652, 195, 116);
        }

        public Rectangle DefineResourcesRectangle()
        {
            return new Rectangle(278, 688, 28, 28);
        }

        public Rectangle DefineBarracksRectangle()
        {
            return new Rectangle(316, 688, 28, 28);
        }

        public Rectangle DefineFactoryRectangle()
        {
            return new Rectangle(354, 688, 28, 28);
        }

        public Rectangle DefineFortressRectangle()
        {
            return new Rectangle(392, 688, 28, 28);
        }
    }
}