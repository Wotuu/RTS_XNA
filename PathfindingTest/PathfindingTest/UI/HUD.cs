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
        public Boolean loadForResources { get; set; }
        public Boolean loadForBarracks { get; set; }
        public Boolean loadForFactory { get; set; }
        public Boolean loadForFortress { get; set; }
        public Boolean draw { get; set; }

        public LinkedList<HUDObject> objects { get; set; }

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

            hudItemDetails = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDItemDetails");

            sf = Game1.GetInstance().Content.Load<SpriteFont>("Fonts/SpriteFont1");

            loadForEngineer = false;
            loadForBarracks = false;
            loadForFactory = false;
            loadForFortress = false;
            draw = false;

            objects = new LinkedList<HUDObject>();

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
            draw = CheckDraw();
            CountUnits();

            float startX = 278;
            float startY = 688;

            objects = new LinkedList<HUDObject>();

            if (loadForEngineer)
            {
                HUDObject resourceObject = new HUDObject(Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDResources"), HUDObject.Type.Resources, startX, startY, color);
                objects.AddLast(resourceObject);
                startX += 38;
                HUDObject barracksObject = new HUDObject(Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDBarracks"), HUDObject.Type.Barracks, startX, startY, color);
                objects.AddLast(barracksObject);
                startX += 38;
                HUDObject factoryObject = new HUDObject(Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDFactory"), HUDObject.Type.Factory, startX, startY, color);
                objects.AddLast(factoryObject);
                startX += 38;
                HUDObject fortressObject = new HUDObject(Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDFortress"), HUDObject.Type.Fortress, startX, startY, color);
                objects.AddLast(fortressObject);
                startX += 38;
            }
            if (loadForFortress)
            {
                HUDObject engineerObject = new HUDObject(Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDEngineer"), HUDObject.Type.Engineer, startX, startY, color);
                objects.AddLast(engineerObject);
                startX += 38;
            }
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

            foreach (HUDObject o in objects)
            {
                o.Draw(sb);
            }
        }

        void MouseClickListener.OnMouseClick(MouseEvent me)
        {
            if (me.button == MouseEvent.MOUSE_BUTTON_1)
                //player.currentSelection != null
                //player.currentSelection.units.Count == 1 &&
                //player.currentSelection.units.ElementAt(0).type == Unit.UnitType.Engineer &&)
            {
                foreach (HUDObject o in objects)
                {
                    if (o.DefineRectangle().Contains(Mouse.GetState().X, Mouse.GetState().Y))
                    {
                        Console.Out.WriteLine("Previewing a building!");
                        player.RemovePreviewBuildings();
                        Building b;
                        Unit u;

                        switch (o.type)
                        {
                            case HUDObject.Type.Resources:
                                b = new ResourceGather(this.player, this.color);
                                break;

                            case HUDObject.Type.Barracks:
                                b = new Barracks(this.player, this.color);
                                break;

                            case HUDObject.Type.Factory:
                                b = new Factory(this.player, this.color);
                                break;

                            case HUDObject.Type.Fortress:
                                b = new Fortress(this.player, this.color);
                                break;

                            case HUDObject.Type.Engineer:
                                foreach (Building building in player.buildingSelection.buildings)
                                {
                                    building.CreateUnit(Unit.UnitType.Engineer);
                                }
                                break;

                            default:
                                break;
                        }

                        Console.Out.WriteLine("Creating a building");
                        // player.buildings.AddLast(b);
                    }
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
                    switch (u.type)
                    {
                        case Unit.UnitType.Engineer:
                            engineerCounter++;
                            break;

                        default:
                            break;
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


            int resourcesCounter = 0;
            int barracksCounter = 0;
            int factoryCounter = 0;
            int fortressCounter = 0;

            if (player.buildingSelection != null)
            {
                foreach (Building b in player.buildingSelection.buildings)
                {
                    switch (b.type)
                    {
                        case Building.BuildingType.Resources:
                            resourcesCounter++;
                            break;

                        case Building.BuildingType.Barracks:
                            barracksCounter++;
                            break;

                        case Building.BuildingType.Factory:
                            factoryCounter++;
                            break;

                        case Building.BuildingType.Fortress:
                            fortressCounter++;
                            break;

                        default:
                            break;
                    }
                }
            }

            if (resourcesCounter > 0)
            {
                loadForResources = true;
            }
            else
            {
                loadForResources = false;
            }

            if (barracksCounter > 0)
            {
                loadForBarracks = true;
            }
            else
            {
                loadForBarracks = false;
            }

            if (factoryCounter > 0)
            {
                loadForFactory = true;
            }
            else
            {
                loadForFactory = false;
            }

            if (fortressCounter > 0)
            {
                loadForFortress = true;
            }
            else
            {
                loadForFortress = false;
            }
        }

        public Boolean CheckDraw()
        {
            if (player.currentSelection != null)
            {
                if (player.currentSelection.units.Count > 0)
                {
                    return true;
                }
            }

            if (player.buildingSelection != null)
            {
                if (player.buildingSelection.buildings.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Whether the mouse is over a building preview text or not.
        /// </summary>
        /// <returns>Yes or no.</returns>
        public Boolean IsMouseOverBuilding()
        {
            Boolean check = false;

            foreach (HUDObject o in objects)
            {
                if (o.DefineRectangle().Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    check = true;
                }
            }

            return check;
        }

        public Rectangle DefineRectangle()
        {
            return new Rectangle(195, 652, 634, 116);
        }

        public Rectangle DefineDetailsRectangle()
        {
            return new Rectangle(0, 652, 195, 116);
        }
    }
}