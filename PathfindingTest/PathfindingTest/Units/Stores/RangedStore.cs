using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Players;

namespace PathfindingTest.Units.Stores
{
    class RangedStore : UnitStore
    {
        public RangedStore(Player player)
        {
            this.player = player;
        }

        protected override Unit createUnit(String type, int x, int y)
        {
            if (type.Equals("bowman"))
            {
                return new Bowman(player, x, y);
            }
            else if (type.Equals("heavy"))
            {
                return null;
            }
            return null;
        }
    }
}
