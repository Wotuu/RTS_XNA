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

        public Boolean selected { get; set; }
        public Boolean canPlace { get; set; }
        public BuildState state { get; set; }

        public BuildingType type { get; set; }
        public Texture2D texture { get; set; }

        public BuildingMesh mesh { get; set; }
        public ProgressBar progressBar { get; set; }

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
            Finished
        }

        public void Update(KeyboardState ks, MouseState ms)
        {
            if (state == BuildState.Preview)
            {
                canPlace = Game1.GetInstance().collision.CanPlace(this.DefineRectangle());
                this.x = (ms.X - (texture.Width / 2));
                this.y = (ms.Y - (texture.Height / 2));
            }
            else if (state == BuildState.Constructing && constructedBy.waypoints.Count == 0)
            {
                if (this.constructC.A < 255)
                {
                    Color newColor = this.constructC;
                    constructProgress += (1 / constructDuration);
                    newColor.A = (byte) ((constructProgress / 100) * 255);
                    this.constructC = newColor;
                }
                else
                {
                    state = BuildState.Finished;
                }
            }
        }

        internal void Draw(SpriteBatch sb)
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

                case BuildState.Finished:
                    sb.Draw(texture, new Vector2(x, y), c);
                    break;

                default:
                    break;
            }

            DrawProgressBar(sb);
        }

        internal void DrawProgressBar(SpriteBatch sb)
        {
            if (this.state == BuildState.Constructing)
            {
                progressBar.progress = this.constructProgress;
                progressBar.Draw(sb);
            }
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
        }
    }
}
