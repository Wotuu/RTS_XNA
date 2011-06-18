using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Buildings;

namespace PathfindingTest.Multiplayer.Data
{
    public class BuildingMultiplayerData : MultiplayerData
    {
        public Building building { get; set; }

        public BuildingMultiplayerData(Building building)
        {
            this.building = building;
        }

        public override int GetUnitType()
        {
            throw new NotImplementedException();
        }
    }
}
