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
using PathfindingTest.Multiplayer.Data;
using SocketLibrary.Protocol;

namespace PathfindingTest.Units.Melee
{
    class Swordman : CombatUnit
    {
        public Swordman(Player p, int x, int y, int baseDamage)
            : base(p, x, y, 1f, 20f, 60)
        {
            this.baseDamage = baseDamage;

            this.type = Type.Melee;

            Console.Out.WriteLine("Constructed a swordsman @ " + this.GetLocation() + " (" + x + ", " + y + ")");

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Units/melee");

            this.productionDuration = 5;
            this.productionProgress = 0;
        }

        public override void Update(KeyboardState ks, MouseState ms)
        {
            if (this.state == State.Finished)
            {
                UpdateMovement();

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

                /*if (this.DefineRectangle().Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    this.DrawHealthBar(sb);
                }*/
            }
        }

        /// <summary>
        /// Attempt to swing the weapon!
        /// </summary>
        public override void Swing()
        {

            //if (this.fireCooldown < 0)
            // {
            CheckForEnemiesInRange();
            if (this.enemiesInRange.Count == 0)
            {
                return;
            }
            // Console.WriteLine("swung weapon");
            Unit targetUnit = this.enemiesInRange.ElementAt(0);
            AggroEvent e = new AggroEvent(this, targetUnit, true);
            targetUnit.OnAggroRecieved(e);
            this.OnAggro(e);
            DamageEvent dmgEvent = new DamageEvent(new MeleeSwing(PathfindingTest.Combat.DamageEvent.DamageType.Melee, baseDamage), targetUnit);
            targetUnit.OnDamage(dmgEvent);
            this.fireCooldown = this.rateOfFire;
            //    }
            //     else
            //     {
            //         Console.WriteLine("Cant fire Q.Q");
            //    }
        }

        /// <summary>
        /// Attempt to fire the weapon!
        /// </summary>
        public override void Swing(Unit unitToAttack)
        {
            if (this.fireCooldown < 0)
            {
                AggroEvent e = new AggroEvent(this, unitToAttack, true);
                unitToAttack.OnAggroRecieved(e);
                this.OnAggro(e);
                //DamageEvent dmgEvent = new DamageEvent(null, unitToAttack);
                //unitToAttack.OnDamage(dmgEvent);
                this.fireCooldown = this.rateOfFire;
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

    }
}
