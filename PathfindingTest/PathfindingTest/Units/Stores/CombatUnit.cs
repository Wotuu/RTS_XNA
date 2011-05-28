using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Players;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Pathfinding;
using PathfindingTest.Units.Projectiles;

namespace PathfindingTest.Units
{
    public abstract class CombatUnit : Unit
    {

        public LinkedList<Unit> enemiesInRange { get; set; }

        public float range { get; set; }
        public float fireCooldown { get; set; }
        public float rateOfFire { get; set; }

        public LinkedList<Projectile> projectiles { get; set; }

        public CombatUnit(Player p, int x, int y, float movementSpeed, float range, float rateOfFire) : 
            base( p, x, y, movementSpeed)
        {
            this.range = range;
            this.rateOfFire = rateOfFire;
            this.enemiesInRange = new LinkedList<Unit>();

            this.projectiles = new LinkedList<Projectile>();
        }

        /// <summary>
        /// Updates the enemiesInRange variable, to contain all the enemies within the range of this unit.
        /// </summary>
        public void CheckForEnemiesInRange()
        {
            enemiesInRange.Clear();
            foreach (Player player in Game1.GetInstance().players)
            {
                // Don't check for units on our alliance
                if (player.alliance.members.Contains(this.player)) continue;
                else
                {
                    foreach (Unit unit in player.units)
                    {
                        if (Util.GetHypoteneuseLength(unit.GetLocation(), this.GetLocation()) < this.range)
                        {
                            enemiesInRange.AddLast(unit);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reduces the fire cooldown of this unit.
        /// </summary>
        public void AttemptReload()
        {
            this.fireCooldown--;
        }

        /// <summary>
        /// This unit will attempt to fire!
        /// </summary>
        public abstract void Fire();
    }
}
