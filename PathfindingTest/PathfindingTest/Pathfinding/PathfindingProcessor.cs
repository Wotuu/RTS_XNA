using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Units;
using Microsoft.Xna.Framework;

namespace PathfindingTest.Pathfinding
{
    public class PathfindingProcessor
    {
        private static PathfindingProcessor instance { get; set; }
        private LinkedList<UnitProcess> toProcess { get; set; }

        /// <summary>
        /// Pushes a unit onto the list.
        /// </summary>
        /// <param name="unit">The unit</param>
        /// <param name="target">The target of the unit</param>
        public void Push(Unit unit, Point target)
        {
            toProcess.AddLast(new UnitProcess(unit, target));
        }

        /// <summary>
        /// Removes a unit from the process list.
        /// </summary>
        /// <param name="unit">The unit.</param>
        public void Remove(Unit unit)
        {
            for( int i = 0; i < toProcess.Count; i++ )
            {
                UnitProcess up = toProcess.ElementAt(i);
                if (up.unit == unit)
                {
                    toProcess.Remove(up);
                    break;
                }
            }
        }

        public void Process()
        {
            if (toProcess.Count != 0)
            {
                UnitProcess up = toProcess.ElementAt(0);
                up.unit.MoveToNow(up.target);
                toProcess.RemoveFirst();
            }
        }


        public static PathfindingProcessor GetInstance()
        {
            if (instance == null) instance = new PathfindingProcessor();
            return instance;
        }

        private PathfindingProcessor()
        {
            toProcess = new LinkedList<UnitProcess>();
        }

        private class UnitProcess
        {
            public Unit unit { get; set; }
            public Point target { get; set; }

            public UnitProcess(Unit unit, Point target)
            {
                this.unit = unit;
                this.target = target;
            }
        }
    }
}
