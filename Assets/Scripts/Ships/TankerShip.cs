using Space;

namespace Ships
{
    /// <summary>
    /// A Tanker ship.
    /// </summary>
    [System.Serializable]
    public class TankerShip : Spaceship
    {
        /// <summary>
        /// The names for each possible order type for this ship. Varies per ship type.
        /// </summary>
        public override string[] OrderTypes
        {
            get
            {
                return new string[] { "Goto", "Refuel", "Unload 50%" };
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_systemLocationIndex">Index of system this ship was built in.</param>
        /// <param name="_initialOrbitLocation">The initial location of this ship.</param>
        public TankerShip(int _systemLocationIndex, Point _initialOrbitLocation) :
            base(ShipType.Tanker, "Tanker (C-" + StateManager.currentSM.currentSession.NumberOfShipsEverBuilt + ")", _systemLocationIndex, _initialOrbitLocation)
        {

        }

        /// <summary>
        /// Get the string representation of an order.
        /// </summary>
        /// <param name="index">The index of the order to be retrieved.</param>
        /// <returns>String representation of the order, complete with type and location.</returns>
        public override string GetOrder(int index)
        {
            int orderType = orders[index].orderType;
            string place = orders[index].point.LocationName;

            if (orderType == 0)
                return "Go to " + place;
            else if (orderType == 1)
                return "Refuel at " + place;
            else if (orderType == 2)
                return "Unload 50% of Fuel at " + place;
            else
                return "Invalid order at " + place;
        }

        /// <summary>
        /// Process the orders. Once per in-game frame. 
        /// </summary>
        public override void OrderUpdate()
        {
            UpdateConditionAndVerifyOrders();

            if (orders.Count == 0) return;
            Order curr = orders[0];

            if (curr.orderType == 2)
            {
                if (orbitingObject == null) orbitingObject = orders[0].point;
                if (orbitingObject is OrbitingBody) {
                    if ((orbitingObject as OrbitingBody).colony == null)
                        UIManager.current.DisplayMessage("No colony on " + orders[0].point.LocationName + " for " + ShipName + " to unload fuel at!");
                    else
                    {
                        (orbitingObject as OrbitingBody).colony.UnloadFuel(FuelOnBoard * 0.5f);
                        FuelOnBoard *= 0.5f;
                    }
                }
                Cycle(); 
            }
            else
            {
                base.OrderUpdate(); 
            }
        }


    }
}
