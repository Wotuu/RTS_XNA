using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Players;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PathfindingTest.Units.Projectiles;
using PathfindingTest.Combat;

namespace PathfindingTest.Units
{
    public class Bowman : CombatUnit
    {
        public Bowman(Player p, int x, int y)
            : base(p, x, y, 1f, 100f, 60)
        {

            this.player = p;
            this.x = x;
            this.y = y;
            this.type = UnitType.Ranged;

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Units/bowman");
        }

        public override void Update(KeyboardState ks, MouseState ms)
        {
            UpdateMovement();
            AttemptReload();
            //5wertyugh
            // Don't do this that often, not really needed.
            if (Game1.GetInstance().frames % 4 == 0)
            {
                // CheckCollision();
                Fire();
            }

            for( int i = 0; i < projectiles.Count; i++ ){
                projectiles.ElementAt(i).Update(ks, ms);
            }
        }

        internal override void Draw(SpriteBatch sb)
        {
            sb.Draw(this.texture, new Vector2(x - (texture.Width / 2), y - (texture.Height / 2)), this.color);

            if (this.DefineRectangle().Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                this.DrawHealthBar(sb);
            }

            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles.ElementAt(i).Draw(sb);
            }
        }

        public override void OnAggroRecieved(AggroEvent e)
        {
            // Console.Out.WriteLine("Recieved aggro from something! D=");
        }

        public override void OnAggro(AggroEvent e)
        {
            // Console.Out.WriteLine("Aggroing something, *grins*");
        }


        /// <summary>
        /// Attempt to fire the weapon!
        /// </summary>
        public override void Fire()
        {
            if (this.fireCooldown < 0)
            {
                CheckForEnemiesInRange();
                if (this.enemiesInRange.Count == 0) { return; }
                Unit targetUnit = this.enemiesInRange.ElementAt(0);
                AggroEvent e = new AggroEvent(this, targetUnit, true);
                targetUnit.OnAggroRecieved(e);
                this.OnAggro(e);
                this.projectiles.AddLast(new Arrow(this, targetUnit));
                this.fireCooldown = this.rateOfFire;
            }
        }
    }
}
