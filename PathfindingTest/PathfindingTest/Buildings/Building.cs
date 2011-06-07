﻿using System;
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
        public State state { get; set; }

        public LinkedList<Unit> productionQueue { get; set; }

        public Type type { get; set; }
        public Texture2D texture { get; set; }

        public BuildingMesh mesh { get; set; }
        public HealthBar healthBar { get; set; }
        public ProgressBar progressBar { get; set; }

        public LinkedList<Unit> constructionQueue { get; set; }
        public Point waypoint { get; set; }

        public enum Type
        {
            Resources,
            Barracks,
            Factory,
            Fortress
        }

        public enum State
        {
            Preview,
            Constructing,
            Interrupted,
            Finished,
            Producing
        }

        public abstract void Update(KeyboardState ks, MouseState ms);

        internal abstract void Draw(SpriteBatch sb);

        public void DefaultUpdate(KeyboardState ks, MouseState ms)
        {
            if (constructedBy != null && state == State.Constructing)
            {
                if (constructedBy.waypoints.Count > 0 && constructionStarted)
                {
                    constructedBy = null;
                    this.state = State.Interrupted;
                }
            }

            switch (state)
            {
                case State.Preview:
                    canPlace = Game1.GetInstance().collision.CanPlace(this.DefineRectangle());
                    this.x = (ms.X - (texture.Width / 2));
                    this.y = (ms.Y - (texture.Height / 2));
                    break;

                case State.Constructing:
                    if (constructedBy.waypoints.Count == 0)
                    {
                        this.constructionStarted = true;

                        if (this.constructC.A < 255)
                        {
                            Color newColor = this.constructC;
                            constructProgress += (1 / constructDuration);
                            newColor.A = (byte)((constructProgress / 100) * 255);

                            float newHealth = (float)((constructProgress / 100) * maxHealth);
                            if (newHealth > maxHealth) newHealth = maxHealth;
                            currentHealth = newHealth;

                            this.constructC = newColor;
                        }
                        else
                        {
                            state = State.Finished;
                        }
                    }
                    break;

                case State.Interrupted:
                    break;

                case State.Finished:
                    if (productionQueue != null)
                    {
                        if (productionQueue.Count > 0)
                        {
                            this.state = State.Producing;
                        }
                    }
                    break;

                case State.Producing:
                    if (productionQueue == null)
                    {
                        this.state = State.Finished;
                        break;
                    }

                    if (productionQueue != null)
                    {
                        if (productionQueue.Count == 0)
                        {
                            this.state = State.Finished;
                            break;
                        }
                        else if (productionQueue.Count > 0)
                        {
                            Produce();
                        }
                    }
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
                case State.Preview:
                    if (canPlace)
                    {
                        sb.Draw(texture, new Vector2(x, y), previewC);
                    }
                    if (!canPlace)
                    {
                        sb.Draw(texture, new Vector2(x, y), previewCantPlace);
                    }
                    break;

                case State.Constructing:
                    sb.Draw(texture, new Vector2(x, y), constructC);
                    break;

                case State.Interrupted:
                    sb.Draw(texture, new Vector2(x, y), constructC);
                    break;

                case State.Finished:
                    sb.Draw(texture, new Vector2(x, y), c);
                    break;

                case State.Producing:
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
            if (this.state == State.Constructing || this.state == State.Interrupted)
            {
                progressBar.progress = this.constructProgress;
                progressBar.Draw(sb);
            }
            else if (this.state == State.Producing)
            {
                Unit produced = productionQueue.ElementAt(0);
                progressBar.progress = produced.productionProgress;
                progressBar.Draw(sb);
            }
        }

        internal void DrawHealthBar(SpriteBatch sb)
        {
            int healthPercent = (int)((this.currentHealth / this.maxHealth) * 100.0);
            healthBar.percentage = healthPercent;
            healthBar.Draw(sb);
        }

        /// <summary>
        /// Places the building on the map
        /// </summary>
        public void PlaceBuilding(Engineer e)
        {
            this.state = State.Constructing;
            this.constructedBy = e;
            this.mesh = Game1.GetInstance().collision.PlaceBuilding(this.DefineSelectedRectangle());
            this.waypoint = new Point((int)this.x + (this.texture.Width / 2), (int)this.y + this.texture.Height + 20);
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

        public void CreateUnit(Unit.Type type)
        {
            Unit newUnit = null;

            switch (type)
            {
                case Unit.Type.Engineer:
                    if (this.type == Type.Fortress)
                    {
                        newUnit = new Engineer(this.p, (int)this.x + (this.texture.Width / 2), (int)this.y + (this.texture.Height / 2));
                        newUnit.state = Unit.State.Producing;
                        productionQueue.AddLast(newUnit);
                    }
                    break;

                case Unit.Type.Melee:
                    break;

                case Unit.Type.HeavyMelee:
                    break;

                case Unit.Type.Fast:
                    break;

                case Unit.Type.Ranged:
                    break;

                default:
                    break;
            }
        }

        public void Produce()
        {
            Unit produced = productionQueue.ElementAt(0);

            produced.productionProgress += (1 / produced.productionDuration);

            if (produced.productionProgress >= 100)
            {
                produced.currentHealth = produced.maxHealth;
                produced.state = Unit.State.Finished;
                produced.x = waypoint.X;
                produced.y = waypoint.Y;
                productionQueue.RemoveFirst();
                this.state = State.Finished;
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

            this.state = State.Preview;
            this.progressBar = new ProgressBar(this);
            this.healthBar = new HealthBar(this);

            this.productionQueue = new LinkedList<Unit>();
        }
    }
}