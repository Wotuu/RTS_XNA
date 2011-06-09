using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Units;
using PathfindingTest.Buildings;
using PathfindingTest.UI;
using PathfindingTest.Selection;
using PathfindingTest.Selection.Patterns;
using PathfindingTest.Pathfinding;
using XNAInputHandler.MouseInput;
using PathfindingTest.Units.Stores;
using System.Diagnostics;

namespace PathfindingTest.Players
{
    public class Player : MouseClickListener, MouseMotionListener
    {

        public Color color;
        public Alliance alliance;

        Texture2D selectionTex;
        Texture2D selectedTex;

        public LinkedList<Unit> units { get; set; }
        public UnitSelection currentSelection { get; set; }
        public LinkedList<Building> buildings { get; set; }
        public BuildingSelection buildingSelection { get; set; }

        public HUD hud { get; set; }

        public Point previewPatternClick { get; set; }
        public UnitGroupPattern previewPattern { get; set; }

        public SelectRectangle selectBox { get; set; }

        private int lastBtn1ClickFrames { get; set; }

        public UnitStore meleeStore;
        public UnitStore rangedStore;
        public UnitStore fastStore;

        /// <summary>
        /// Player constructor.
        /// </summary>
        /// <param name="color"></param>
        public Player(Alliance alliance, Color color)
        {
            this.alliance = alliance;
            if (!this.alliance.members.Contains(this)) this.alliance.members.AddLast(this);
            this.color = color;

            selectionTex = Game1.GetInstance().Content.Load<Texture2D>("Selection");
            selectedTex = Game1.GetInstance().Content.Load<Texture2D>("Selected");

            units = new LinkedList<Unit>();
            buildings = new LinkedList<Building>();
            hud = new HUD(this, color);

            meleeStore = new MeleeStore(this);
            rangedStore = new RangedStore(this);
            fastStore = new FastStore(this);

            MouseManager.GetInstance().mouseClickedListeners += ((MouseClickListener)this).OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners += ((MouseClickListener)this).OnMouseRelease;
            MouseManager.GetInstance().mouseMotionListeners += ((MouseMotionListener)this).OnMouseMotion;
            MouseManager.GetInstance().mouseDragListeners += ((MouseMotionListener)this).OnMouseDrag;
        }

        /// <summary>
        /// Spawns the starting units of this player.
        /// </summary>
        /// <param name="location">The location to spawn them at.</param>
        public void SpawnStartUnits(Point location)
        {
            LinkedList<Unit> temp_units = new LinkedList<Unit>();
            for (int i = 0; i < 15; i++)
            {
                if (i % 2 == 0)
                {
                    temp_units.AddLast(meleeStore.getUnit(Unit.Type.Melee, 0, 0, 5));
                } else
                {
                    temp_units.AddLast(rangedStore.getUnit(Unit.Type.Ranged, 0, 0, 5));
                }
            }

            temp_units.AddLast(meleeStore.getUnit(Unit.Type.Engineer, 0, 0, 1));

            UnitSelection selection = new UnitSelection(temp_units);

            UnitGroupPattern pattern = new CirclePattern(location, selection, 90, 0);

            LinkedList<Point> points = pattern.ApplyPattern();

            for (int i = 0; i < temp_units.Count; i++)
            {
                temp_units.ElementAt(i).x = points.ElementAt(i).X;
                temp_units.ElementAt(i).y = points.ElementAt(i).Y;
            }
        }

        public UnitSelection GetSelectedUnits()
        {
            UnitSelection selection = new UnitSelection();

            foreach (Unit unit in this.units)
            {
                Console.WriteLine("Unit added");
                if (unit.selected) selection.units.AddLast(unit);
            }

            return selection;
        }

        public BuildingSelection GetSelectedBuildings()
        {
            BuildingSelection selection = new BuildingSelection();

            foreach (Building b in this.buildings)
            {
                if (b.selected)
                {
                    selection.buildings.AddLast(b);
                }
            }

            return selection;
        }

        public void DeselectAllUnits()
        {
            foreach (Unit unit in this.units)
            {
                unit.selected = false;
            }
        }

        public void DeselectAllBuildings()
        {
            foreach (Building b in this.buildings)
            {
                b.selected = false;
            }
        }

        /// <summary>
        /// Standard Update function.
        /// </summary>
        /// <param name="ks"></param>
        /// <param name="ms"></param>
        public void Update(KeyboardState ks, MouseState ms)
        {
            foreach (Unit u in units)
            {
                u.Update(ks, ms);
            }

            foreach (Building b in buildings)
            {
                b.Update(ks, ms);
            }

            // Show healthbar over units that mouse is hovering over
            Boolean selectedAUnit = false;
            foreach (Player p in Game1.GetInstance().players)
            {
                foreach (Unit u in p.units)
                {
                    if (!selectedAUnit && u.DefineRectangle().Contains(ms.X, ms.Y))
                    {
                        u.selected = true;
                        selectedAUnit = true;
                    }
                    else if (this.currentSelection == null || !this.currentSelection.units.Contains(u))
                    {
                        u.selected = false;
                    }
                }
            }

            hud.Update(ks, ms);
        }

        /// <summary>
        /// Standard Draw function.
        /// </summary>
        /// <param name="sb"></param>
        internal void Draw(SpriteBatch sb)
        {
            if (selectBox != null) selectBox.Draw(sb);

            if (previewPattern != null)
            {
                LinkedList<Point> points = previewPattern.ApplyPattern();
                foreach (Point p in points)
                {
                    new PatternPreviewObject(p).Draw(sb);
                }
            }

            foreach (Unit u in units)
            {
                u.Draw(sb);
            }
            foreach (Unit u in units)
            {
                u.Draw(sb);
            }

            if (currentSelection != null)
            {
                foreach (Unit uH in currentSelection.units)
                {
                    uH.DrawHealthBar(sb);
                }
            }
            foreach (Building b in buildings)
            {
                b.Draw(sb);
            }

            hud.Draw(sb);
        }

        /// <summary>
        /// Whether the player is currently previewing a building
        /// </summary>
        /// <returns>Yes or no.</returns>
        public Boolean IsPreviewingBuilding()
        {
            foreach (Building b in buildings)
            {
                if (b.state == Building.State.Preview) return true;
            }
            return false;
        }

        /// <summary>
        /// Whether the mouse is over a unit or not
        /// if it is, it'll return it. =D
        /// </summary>
        /// <returns>The unit, or null if there was no unit!</returns>
        public Unit getMouseOverUnit(LinkedList<Unit> units)
        {
            foreach (Unit u in units)
            {
                if (u.DefineRectangle().Contains(Mouse.GetState().X, Mouse.GetState().Y)) return u;
            }
            return null;
        }

        public Building IsMouseOverFriendlyBuilding()
        {
            foreach (Building b in buildings)
            {
                if (b.DefineRectangle().Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    return b;
                }
            }

            return null;
        }


        public UnitSelection GetUnits()
        {
            UnitSelection selection = new UnitSelection(new LinkedList<Unit>());
            foreach (Unit unit in units)
            {
                if (this.selectBox.GetRectangle().Contains((int)unit.x, (int)unit.y))
                {
                    selection.units.AddLast(unit);
                }
            }
            return selection;
        }

        public BuildingSelection GetBuildings()
        {
            BuildingSelection selection = new BuildingSelection(new LinkedList<Building>());

            foreach (Building b in this.buildings)
            {
                if (this.selectBox.GetRectangle().Contains((int)b.x, (int)b.y))
                {
                    selection.buildings.AddLast(b);
                }
            }

            return selection;
        }

        /// <summary>
        /// Removes all the preview buildings
        /// </summary>
        public void RemovePreviewBuildings()
        {
            for (int i = 0; i < this.buildings.Count; i++)
            {
                Building build = this.buildings.ElementAt(i);
                if (build.state == Building.State.Preview)
                {
                    build.Dispose();
                    i--;
                }
            }
        }

        public void OnMouseClick(MouseEvent m)
        {
            // Bots dont use the mouse, or shouldn't
            if (Game1.CURRENT_PLAYER != this)
            {
                return;
            }

            if ((m.button == MouseEvent.MOUSE_BUTTON_3))
            {
                if (IsPreviewingBuilding())
                {
                    this.RemovePreviewBuildings();
                }
                else
                {
                    previewPatternClick = m.location;
                }
            }


            if (!hud.DefineRectangle().Contains(new Rectangle(m.location.X, m.location.Y, 1, 1)))
            {
                if (m.button == MouseEvent.MOUSE_BUTTON_1)
                {
                    Unit mouseOverUnit = this.getMouseOverUnit(this.units);
                    if (mouseOverUnit == null)
                    {
                        if (this.currentSelection != null && this.currentSelection.units.Count != 0 &&
                            !this.hud.IsMouseOverBuilding() && !this.IsPreviewingBuilding())
                        {
                            this.DeselectAllUnits();
                            this.currentSelection = null;
                        }
                    }
                    else
                    {
                        this.DeselectAllUnits();
                        this.DeselectAllBuildings();
                        // Performed a double click!
                        if (Game1.GetInstance().frames - lastBtn1ClickFrames < 20)
                        {
                            LinkedList<Unit> selectionUnits = new LinkedList<Unit>();
                            foreach (Unit unit in units)
                            {
                                if (mouseOverUnit.type == unit.type) selectionUnits.AddLast(unit);
                            }
                            this.currentSelection = new UnitSelection(selectionUnits);
                        }
                        else if (!mouseOverUnit.selected)
                        {
                            LinkedList<Unit> selectionUnits = new LinkedList<Unit>();
                            selectionUnits.AddLast(mouseOverUnit);
                            this.currentSelection = new UnitSelection(selectionUnits);
                        }
                    }

                    Building mouseOverBuilding = this.IsMouseOverFriendlyBuilding();
                    if (mouseOverBuilding == null)
                    {
                        if (this.buildingSelection != null && this.buildingSelection.buildings.Count != 0 &&
                            !this.IsPreviewingBuilding())
                        {
                            this.DeselectAllBuildings();
                            this.buildingSelection = null;
                        }
                    }
                    else
                    {
                        this.DeselectAllUnits();
                        this.DeselectAllBuildings();
                        // Performed a double click!
                        if (Game1.GetInstance().frames - lastBtn1ClickFrames < 20)
                        {
                            LinkedList<Building> selectionBuildings = new LinkedList<Building>();
                            foreach (Building b in buildings)
                            {
                                if (mouseOverBuilding.type == b.type)
                                {
                                    selectionBuildings.AddLast(b);
                                }
                            }
                            this.buildingSelection = new BuildingSelection(selectionBuildings);
                        }
                        else if (!mouseOverBuilding.selected)
                        {
                            LinkedList<Building> selectionBuildings = new LinkedList<Building>();
                            selectionBuildings.AddLast(mouseOverBuilding);
                            this.buildingSelection = new BuildingSelection(selectionBuildings);
                            this.buildingSelection.SelectAll();
                        }
                    }
                }
            }

            if (m.button == MouseEvent.MOUSE_BUTTON_1)
            {
                lastBtn1ClickFrames = Game1.GetInstance().frames;
            }

        }

        public void OnMouseRelease(MouseEvent m)
        {
            // Bots dont use the mouse, or shouldn't
            if (Game1.CURRENT_PLAYER != this) return;
            if (selectBox != null && !IsPreviewingBuilding())
            {
                this.currentSelection = this.GetUnits();
                this.currentSelection.SelectAll();

                if (currentSelection.units.Count == 0)
                {
                    this.buildingSelection = this.GetBuildings();
                    this.buildingSelection.SelectAll();
                }
            }
            selectBox = null;
            ///unitz
            if (this.currentSelection != null && this.currentSelection.units.Count != 0
                && (m.button == MouseEvent.MOUSE_BUTTON_3))
            {
                foreach (Player player in Game1.GetInstance().players)
                {
                    if (player.alliance.members.Contains(this))
                    {
                        Unit selectedFriendly = getMouseOverUnit(player.units);
                        if (selectedFriendly != null)
                        {
                            Console.WriteLine("PROTECT");
                            //@todo start protect.
                        }
                    }
                    else
                    {
                        Unit selectedEnemy = getMouseOverUnit(player.units);
                        if (selectedEnemy != null)
                        {
                            foreach (Unit unit in currentSelection.units)
                            {
                                unit.Attack(selectedEnemy);
                            }
                        }
                        else
                        {
                            if (previewPattern != null)
                            {
                                this.currentSelection.MoveTo(previewPattern);
                            }
                            // If we're suppose to move in the first place
                            else this.currentSelection.MoveTo(GetNewPreviewPattern(m.location, 0));
                        }
                        previewPattern = null;

                    }
                }
            }
        }

        public void OnMouseMotion(MouseEvent m)
        {
            // Bots dont use the mouse, or shouldn't
            if (Game1.CURRENT_PLAYER != this) return;
            //throw new NotImplementedException();
        }

        public void OnMouseDrag(MouseEvent m)
        {
            // Bots dont use the mouse, or shouldn't
            if (Game1.CURRENT_PLAYER != this) return;
            previewPattern = null;
            if (m.button == MouseEvent.MOUSE_BUTTON_1)
            {
                if (selectBox == null)
                {
                    selectBox = new SelectRectangle();
                    selectBox.clickedPoint = m.location;
                }
                else
                {
                    int x = selectBox.clickedPoint.X;
                    int y = selectBox.clickedPoint.Y;
                    selectBox.SetRectangle(new Rectangle(x, y, m.location.X - x, m.location.Y - y));
                }
            }
            else if (m.button == MouseEvent.MOUSE_BUTTON_3)
            {
                if (currentSelection != null)
                {
                    /*previewPattern = new CirclePattern(previewPatternClick,
                        currentSelection,
                        Math.Max(currentSelection.units.Count * 5, (int)Util.GetHypoteneuseLength(e.location, previewPatternClick)),
                        angle);*/
                    previewPattern = GetNewPreviewPattern(m.location, (int)Util.GetHypoteneuseAngleDegrees(m.location, previewPatternClick));
                    /*previewPattern = new RectanglePattern(previewPatternClick,
                        currentSelection, 5,
                        Math.Max((int)(Util.GetHypoteneuseLength(m.location, previewPatternClick) / 2.0), 30),
                        angle);*/
                }
            }
        }

        /// <summary>
        /// Gets the new current preview pattern (TEMPORARY FUNCTION)
        /// </summary>
        /// <param name="location">The location</param>
        /// <param name="angle">The angle</param>
        /// <returns>A pattern</returns>
        public UnitGroupPattern GetNewPreviewPattern(Point location, int angle)
        {
            return new RectanglePattern(previewPatternClick,
                        currentSelection, (int)Math.Ceiling(Math.Sqrt(currentSelection.units.Count)),
                        Math.Max((int)(Util.GetHypoteneuseLength(location, previewPatternClick) / 2.0), 30),
                        angle);
        }
    }
}
