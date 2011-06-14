using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Players;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using PathfindingTest.Combat;
using PathfindingTest.Units.Damage;

namespace PathfindingTest.Units.Melee
{
    class Swordman : CombatUnit
    {
        public Swordman(Player p, int x, int y, int baseDamage)
            : base(p, x, y, 1.25f, 20f, 100f, 60)
        {
            this.baseDamage = baseDamage;

            this.player = p;
            this.x = x;
            this.y = y;
            this.type = Type.Melee;

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Units/melee");

            this.productionDuration = 5;
            this.productionProgress = 0;
        }

        public override void Update(KeyboardState ks, MouseState ms)
        {
            if (this.state == State.Finished)
            {
                UpdateMovement();
            UpdateTarget();

                // Don't do this that often, not really needed.
                if (Game1.GetInstance().frames % 4 == 0)
                {
                    // CheckCollision();
                    Swing();
                }
            }
        }

        internal override void Draw(SpriteBatch sb)
        {
            if (this.state == State.Finished)
            {
                sb.Draw(this.texture, new Vector2(x - (texture.Width / 2), y - (texture.Height / 2)), this.color);

                if (this.DefineRectangle().Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    this.DrawHealthBar(sb);
                }
            }
        }

        /// <summary>
        /// Attempt to swing the weapon!
        /// </summary>
        public override void Swing()
        {
            CheckForEnemiesInRange(this.aggroRange);
            if (unitToStalk == null && enemiesInRange.Count < 1)
            {
                return;
            }
            else if (unitToStalk == null && enemiesInRange.Count > 0)
            {
                unitToStalk = this.enemiesInRange.ElementAt(0);
                this.waypoints.Clear();
            }

            CheckForEnemiesInRange(this.attackRange);
            if (this.enemiesInRange.Contains(unitToStalk))
            {
                this.waypoints.Clear();
            }
            else
            {
                Point p = new Point((int)unitToStalk.x, (int)unitToStalk.y);
                this.MoveToQueue(p);
                return;
            }
            AggroEvent e = new AggroEvent(this, unitToStalk, true);
            unitToStalk.OnAggroRecieved(e);
            this.OnAggro(e);
            DamageEvent dmgEvent = new DamageEvent(new MeleeSwing(PathfindingTest.Combat.DamageEvent.DamageType.Melee, baseDamage), unitToStalk, this);
            unitToStalk.OnDamage(dmgEvent);
            this.fireCooldown = this.rateOfFire;
        }

        public override void OnAggroRecieved(AggroEvent e)
        {
            if (unitToStalk == null)
            {
                unitToStalk = e.from;
            }
            //Console.Out.WriteLine("Recieved aggro from something! D=");
        }

        public override void OnAggro(AggroEvent e)
        {
            // Console.Out.WriteLine("Aggroing something, *grins*");
        }

    }
}
