﻿using System;
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
            : base(p, x, y, 1.25f, 20f, 100f, 60)
        {
            this.baseDamage = baseDamage;

            this.player = p;
            this.x = x;
            this.y = y;
            this.type = Type.Melee;

            Console.Out.WriteLine("Constructed a swordsman @ " + this.GetLocation() + " (" + x + ", " + y + ")");

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Units/melee");

            this.productionDuration = 5;
            this.productionProgress = 0;

            if (Game1.GetInstance().IsMultiplayerGame())
            {
                Boolean isLocal = this.player == Game1.CURRENT_PLAYER;
                this.multiplayerData = new UnitMultiplayerData(this, isLocal);
                if (isLocal)
                {
                    this.multiplayerData.RequestServerID(UnitHeaders.TYPE_SWORDMAN);
                }
            }
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
            
            if (unitToStalk == null && enemiesInRange.Count < 1)
           // {
                CheckForEnemiesInRange();
                if (this.enemiesInRange.Count == 0)
            {
                return;
            }
                Console.WriteLine("swung weapon");
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
