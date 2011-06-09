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
        /// Sets the textures to use for the HUD.
        /// Creates new instances of needed components.
        /// Sets variables to their default values.
        /// </summary>
        /// <param name="p">The player this HUD belongs to</param>
        /// <param name="c">The desired color for this HUD</param>
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
        /// Checks whether the HUD should be hidden or not.
        /// Loads the objects to display according to selected units/buildings.
        /// </summary>
        /// <param name="ks">Default KeyboardState</param>
        /// <param name="ms">Default MouseState</param>
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
                startY = SetStartY(startX, startY);
                startX = SetStartX(startX);

                HUDObject barracksObject = new HUDObject(Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDBarracks"), HUDObject.Type.Barracks, startX, startY, color);
                objects.AddLast(barracksObject);
                startY = SetStartY(startX, startY);
                startX = SetStartX(startX);

                HUDObject factoryObject = new HUDObject(Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDFactory"), HUDObject.Type.Factory, startX, startY, color);
                objects.AddLast(factoryObject);
                startY = SetStartY(startX, startY);
                startX = SetStartX(startX);

                HUDObject fortressObject = new HUDObject(Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDFortress"), HUDObject.Type.Fortress, startX, startY, color);
                objects.AddLast(fortressObject);
                startY = SetStartY(startX, startY);
                startX = SetStartX(startX);
            }
            if (loadForBarracks)
            {
                HUDObject meleeObject = new HUDObject(Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDMelee"), HUDObject.Type.Melee, startX, startY, color);
                objects.AddLast(meleeObject);
                startY = SetStartY(startX, startY);
                startX = SetStartX(startX);

                HUDObject rangedObject = new HUDObject(Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDRanged"), HUDObject.Type.Ranged, startX, startY, color);
                objects.AddLast(rangedObject);
                startY = SetStartY(startX, startY);
                startX = SetStartX(startX);
            }
            if (loadForFortress)
            {
                HUDObject engineerObject = new HUDObject(Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDEngineer"), HUDObject.Type.Engineer, startX, startY, color);
                objects.AddLast(engineerObject);
                startY = SetStartY(startX, startY);
                startX = SetStartX(startX);
            }
        }

        public float SetStartY(float startX, float startY)
        {
            float newStartY = startY;

            if (startX == 658)
            {
                newStartY = 726;
            }

            return newStartY;
        }

        public float SetStartX(float startX)
        {
            float newStartX = startX;

            if (newStartX == 658)
            {
                newStartX = 278;
            }
            else
            {
                newStartX += 38;
            }

            return newStartX;
        }

        /// <summary>
        /// Draws the HUD and the objects required.
        /// </summary>
        /// <param name="sb">Default SpriteBatch</param>
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

        /// <summary>
        /// Used for creating units and buildings respectively.
        /// </summary>
        /// <param name="me">The MouseEvent to use</param>
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
                                foreach (Fortress building in player.buildingSelection.buildings)
                                {
                                    building.CreateUnit(Unit.Type.Engineer);
                                }
                                break;

                            case HUDObject.Type.Ranged:
                                foreach (Barracks building in player.buildingSelection.buildings)
                                {
                                    building.CreateUnit(Unit.Type.Ranged);
                                }
                                break;

                            case HUDObject.Type.Melee:
                                foreach (Barracks building in player.buildingSelection.buildings)
                                {
                                    building.CreateUnit(Unit.Type.Melee);
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }

                foreach (Building b in player.buildings)
                {
                    if (b.state == Building.State.Preview &&
                        !this.DefineRectangle().Contains(new Rectangle(me.location.X, me.location.Y, 1, 1)) &&
                        Game1.GetInstance().collision.CanPlace(b.DefineRectangle()))
                    {
                        Engineer temp = null;

                        foreach (Unit u in player.currentSelection.units)
                        {
                            if (u.type == Unit.Type.Engineer)
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

        /// <summary>
        /// MouseRelease Listener.
        /// </summary>
        /// <param name="me">The MouseEvent to use</param>
        void MouseClickListener.OnMouseRelease(MouseEvent me)
        {
            // Not Implemented
        }

        /// <summary>
        /// Count the different unit/building types selected.
        /// Checks what objects to load in the HUD.
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
                        case Unit.Type.Engineer:
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
                        case Building.Type.Resources:
                            resourcesCounter++;
                            break;

                        case Building.Type.Barracks:
                            barracksCounter++;
                            break;

                        case Building.Type.Factory:
                            factoryCounter++;
                            break;

                        case Building.Type.Fortress:
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

        /// <summary>
        /// Checks whether the HUD should be drawn or not.
        /// </summary>
        /// <returns>Returns false if there are no selected units/buildings, else returns true</returns>
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
        /// Checks whether the mouse is hovering over an object on the HUD.
        /// </summary>
        /// <returns>Returns false if not true, else returns true</returns>
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

        /// <summary>
        /// Defines the space the HUD is using.
        /// </summary>
        /// <returns>Returns a Rectangle with the right size</returns>
        public Rectangle DefineRectangle()
        {
            return new Rectangle(195, 652, 634, 116);
        }

        /// <summary>
        /// Defines the space the Details section of the HUD is using.
        /// </summary>
        /// <returns>Returns a Rectangle with the right size</returns>
        public Rectangle DefineDetailsRectangle()
        {
            return new Rectangle(0, 652, 195, 116);
        }
    }
}