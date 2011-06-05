using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PathfindingTest.Buildings;
using PathfindingTest.Players;
using PathfindingTest.Pathfinding;
using PathfindingTest.Collision;
using PathfindingTest.Units;
using System.Diagnostics;

namespace PathfindingTest.Buildings
{
    public abstract class Building
    {

        public Player p { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public Color c { get; set; }
        public Color previewC = new Color(175, 175, 175, 80);
        public Color previewCantPlace = new Color(10, 10, 10, 80);
        public Color constructC { get; set; }
        public Engineer constructedBy { get; set; }
        public double constructDuration { get; set; }
        public double constructProgress { get; set; }

        public float currentHealth { get; set; }
        public float maxHealth { get; set; }

        public Boolean selected { get; set; }
        public Boolean mouseOver { get; set; }
        public Boolean canPlace { get; set; }
        public Boolean constructionStarted { get; set; }
        public BuildState state { get; set; }

        public BuildingType type { get; set; }
        public Texture2D texture { get; set; }

        public BuildingMesh mesh { get; set; }
        public HealthBar healthBar { get; set; }
        public ProgressBar progressBar { get; set; }

        public LinkedList<Unit> constructionQueue { get; set; }

        public enum BuildingType
        {
            Resources,
            Barracks,
            Factory,
            Fortress
        }

        public enum BuildState
        {
            Preview,
            Constructing,
            InterruptedConstruction,
            Finished,
            Producing
        }

        public abstract void Update(KeyboardState ks, MouseState ms);

        internal abstract void Draw(SpriteBatch sb);

        public void DefaultUpdate(KeyboardState ks, MouseState ms)
        {
            if (constructedBy != null && state != BuildState.Finished)
            {
                if (constructedBy.waypoints.Count > 0 && constructionStarted)
                {
                    constructedBy = null;
                    this.state = BuildState.InterruptedConstruction;
                }
            }

            switch (state)
            {
                case BuildState.Preview:
                    canPlace = Game1.GetInstance().collision.CanPlace(this.DefineRectangle());
                    this.x = (ms.X - (texture.Width / 2));
                    this.y = (ms.Y - (texture.Height / 2));
                    break;

                case BuildState.Constructing:
                    if (constructedBy.waypoints.Count == 0)
                    {
                        this.constructionStarted = true;

                        if (this.constructC.A < 255)
                        {
                            Color newColor = this.constructC;
                            constructProgress += (1 / constructDuration);
                            newColor.A = (byte)((constructProgress / 100) * 255);

                            float newHealth = (float) ((constructProgress / 100) * maxHealth);
                            if (newHealth > maxHealth) newHealth = maxHealth;
                            currentHealth = newHealth;

                            this.constructC = newColor;
                        }
                        else
                        {
                            state = BuildState.Finished;
                        }
                    }
                    break;

                case BuildState.InterruptedConstruction:
                    break;

                case BuildState.Finished:
                    break;

                case BuildState.Producing:
                    break;

                default:
                    break;
            }

            if (this.DefineRectangle().Contains(ms.X, ms.Y))
            {
                this.mouseOver = true;
            }
            else
            {
                this.mouseOver = false;
            }
        }

        internal void DefaultDraw(SpriteBatch sb)
        {
            switch (state)
            {
                case BuildState.Preview:
                    if (canPlace)
                    {
                        sb.Draw(texture, new Vector2(x, y), previewC);
                    }
                    if (!canPlace)
                    {
                        sb.Draw(texture, new Vector2(x, y), previewCantPlace);
                    }
                    break;

                case BuildState.Constructing:
                    sb.Draw(texture, new Vector2(x, y), constructC);
                    break;

                case BuildState.InterruptedConstruction:
                    sb.Draw(texture, new Vector2(x, y), constructC);
                    break;

                case BuildState.Finished:
                    sb.Draw(texture, new Vector2(x, y), c);
                    break;

                default:
                    break;
            }

            DrawProgressBar(sb);
            if (selected || mouseOver)
            {
                DrawHealthBar(sb);
            }
        }

        internal void DrawProgressBar(SpriteBatch sb)
        {
            if (this.state == BuildState.Constructing || this.state == BuildState.InterruptedConstruction)
            {
                progressBar.progress = this.constructProgress;
                progressBar.Draw(sb);
            }
        }

        internal void DrawHealthBar(SpriteBatch sb)
        {
            int healthPercent = (int) ((this.currentHealth / this.maxHealth) * 100.0);
            healthBar.percentage = healthPercent;
            healthBar.Draw(sb);
        }

        /// <summary>
        /// Places the building on the map
        /// </summary>
        public void PlaceBuilding(Engineer e)
        {
            this.state = BuildState.Constructing;
            this.constructedBy = e;
            this.mesh = Game1.GetInstance().collision.PlaceBuilding(this.DefineSelectedRectangle());
        }

        /// <summary>
        /// Gets the radius of the circle surrounding this building
        /// </summary>
        /// <returns>The value</returns>
        public int GetCircleRadius()
        {
            return (int)(Util.GetHypoteneuseLength(
                new Point((int)this.x, (int)this.y), 
                new Point(DefineRectangle().Left, DefineRectangle().Bottom)) / 2);
        }

        public void CreateUnit(Unit.UnitType type)
        {
            Unit newUnit;

            switch (type)
            {
                case Unit.UnitType.Engineer:
                    if (this.type == BuildingType.Fortress)
                    {
                        newUnit = new Engineer(this.p, (int)this.x - 70, (int)this.y - 70);
                        p.units.AddLast(newUnit);
                    }
                    break;

                case Unit.UnitType.Melee:
                    break;

                case Unit.UnitType.HeavyMelee:
                    break;

                case Unit.UnitType.Fast:
                    break;

                case Unit.UnitType.Ranged:
                    break;

                default:
                    break;
            }
        }

        public Rectangle DefineRectangle()
        {
            return new Rectangle((int)this.x, (int)this.y, texture.Width, texture.Height);
        }

        public Rectangle DefineSelectedRectangle()
        {
            return new Rectangle((int)this.x - 3, (int)this.y - 3, texture.Width + 6, texture.Height + 6);
        }

        /// <summary>
        /// Disposes of this building
        /// </summary>
        public void Dispose()
        {
            p.buildings.Remove(this);
            if (this.mesh != null) mesh.Reverse();
        }

        public Building(Player player)
        {
            this.p = player;
            this.p.buildings.AddLast(this);
            this.constructProgress = 0;

            this.state = BuildState.Preview;
            this.progressBar = new ProgressBar(this);
            this.healthBar = new HealthBar(this);
        }
    }
}
