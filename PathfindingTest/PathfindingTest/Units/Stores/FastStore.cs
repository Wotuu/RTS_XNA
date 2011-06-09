using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Players;

namespace PathfindingTest.Units.Stores
{
    class FastStore : UnitStore
    {
        public FastStore(Player player)
        {
            this.player = player;
        }

        protected override Unit createUnit(Unit.Type type, int x, int y, int baseDamage)
        {
            return null;
        }
    }
}
