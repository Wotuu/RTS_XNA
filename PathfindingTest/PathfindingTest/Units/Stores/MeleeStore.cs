using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Units.Stores;
using PathfindingTest.Players;

namespace PathfindingTest.Units
{
    public class MeleeStore : UnitStore
    {

        public MeleeStore(Player player)
        {
            this.player = player;
        }

        protected override Unit createUnit(String type, int x, int y)
        {
            if (type.Equals("normal"))
            {
                return null;
            }
            else if (type.Equals("heavy"))
            {
                return null;
            }
            else if (type.Equals("engineer"))
            {
                return new Engineer(player, x, y);
            }
            return null;
        }
    }
}
