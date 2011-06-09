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

        public float fireCooldown { get; set; }
        public float rateOfFire { get; set; }

        public LinkedList<Projectile> projectiles { get; set; }

        public CombatUnit(Player p, int x, int y, float movementSpeed, float range,float rateOfFire) : 
            base( p, x, y, movementSpeed, range)
        {
            this.rateOfFire = rateOfFire;
            this.enemiesInRange = new LinkedList<Unit>();

            this.projectiles = new LinkedList<Projectile>();
        }
    }
}
