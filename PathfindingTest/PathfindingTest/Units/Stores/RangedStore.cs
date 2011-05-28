using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfindingTest.Units.Stores
{
    class RangedStore
    {
        public Unit getUnit(String type)
        {
            return createUnit(type);
        }

        protected abstract Unit createUnit(String type);
    }
}
