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

        public HUD hud { get; set; }

        public Point previewPatternClick { get; set; }
        public UnitGroupPattern previewPattern { get; set; }

        public SelectRectangle selectBox { get; set; }

        private int lastBtn1ClickFrames { get; set; }

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
            for (int i = 0; i < 25; i++)
            {
                if( i % 2 == 0 ) temp_units.AddLast(new Engineer(this, 0, 0));
                else 
                    temp_units.AddLast(new Bowman(this, 0, 0));
            }
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
                if (unit.selected) selection.units.AddLast(unit);
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
                if (b.state == Building.BuildState.Preview) return true;
            }
            return false;
        }

        /// <summary>
        /// Whether the mouse is over a unit or not.
        /// </summary>
        /// <returns>The unit, or null if there was no unit!</returns>
        public Unit IsMouseOverFriendlyUnit()
        {
            foreach (Unit u in units)
            {
                if (u.DefineRectangle().Contains(Mouse.GetState().X, Mouse.GetState().Y)) return u;
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

        /// <summary>
        /// Removes all the preview buildings
        /// </summary>
        public void RemovePreviewBuildings()
        {
            for (int i = 0; i < this.buildings.Count; i++)
            {
                Building build = this.buildings.ElementAt(i);
                if (build.state == Building.BuildState.Preview)
                {
                    build.Dispose();
                    i--;
                }
            }
        }

        public void OnMouseClick(MouseEvent m)
        {
            // Bots dont use the mouse, or shouldn't
            if (Game1.CURRENT_PLAYER != this) return;
            if (!hud.DefineRectangle().Contains(new Rectangle(m.location.X, m.location.Y, 1, 1)))
            {
                if (m.button == MouseEvent.MOUSE_BUTTON_1)
                {
                    Unit mouseOverUnit = this.IsMouseOverFriendlyUnit();
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
                        // Performed a double click!
                        if (Game1.GetInstance().frames - lastBtn1ClickFrames < 30)
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
                }
            }
            if (m.button == MouseEvent.MOUSE_BUTTON_3)
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
            }
            selectBox = null;

            if (m.button == MouseEvent.MOUSE_BUTTON_3 && this.currentSelection != null)
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
