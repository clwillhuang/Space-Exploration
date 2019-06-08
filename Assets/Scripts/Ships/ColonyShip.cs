using Economy;
using Space;

namespace Ships
{
    /// <summary>
    /// A colony ship.
    /// </summary>
    [System.Serializable]
    public class ColonyShip : Spaceship
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_systemLocationIndex">Index of system this ship was built in.</param>
        /// <param name="_initialOrbitLocation">The initial location of this ship.</param>
        public ColonyShip(int _systemLocationIndex, Point _initialOrbitLocation) :
            base(ShipType.Colony, "Colony Ship (C-" + StateManager.currentSM.currentSession.NumberOfShipsEverBuilt + ")", _systemLocationIndex, _initialOrbitLocation)
        {
            ColonistCapacity = 100000;
            ColonistOnBoard = 0; 
        }

        /// <summary>
        /// The maximum amount of colonists this ship can hold.
        /// </summary>
        public readonly int ColonistCapacity;

        /// <summary>
        /// The current number of colonists aboard the ship.
        /// </summary>
        public int ColonistOnBoard { get; private set; }

        /// <summary>
        /// The names for each possible order type for this ship. Varies per ship type.
        /// </summary>
        public override string[] OrderTypes
        {
            get
            {
                return new string[] { "Goto", "Refuel", "Load Colonists", "Unload Colonists" };
            }
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
                return "Load colonists at " + place;
            else if (orderType == 3)
                return "Unload colonists at " + place;
            else
                return "Invalid order at " + place;
        }

        /// <summary>
        /// Process the orders. Once per in-game frame. 
        /// </summary>
        public override void OrderUpdate()
        {
            UpdateConditionAndVerifyOrders(); 
            if (orders.Count == 0 || orders[0].orderType < 2) base.OrderUpdate();
            // load colonists
            else if (orders[0].orderType == 2)
            {
                if (orders[0].point is OrbitingBody)
                {
                    Colony c = (orders[0].point as OrbitingBody).colony;
                    if (c != null)
                    {
                        int colonistsToLoad = ColonistCapacity - ColonistOnBoard;
                        ColonistOnBoard += c.LoadColonists(colonistsToLoad);
                    }
                    else
                    {
                        UIManager.current.DisplayMessage("No colony on " + orders[0].point.LocationName + " for " + ShipName + " to load colonists at!");
                    }
                }
                Cycle();

            }
            // unload colonists
            else if (orders[0].orderType == 3)
            {
                if (orders[0].point is OrbitingBody)
                {
                    Colony c = (orders[0].point as OrbitingBody).colony;

                    if (c != null)
                    {
                        if (c.isHabitable)
                            ColonistOnBoard -= c.UnloadColonists(ColonistOnBoard);
                        else
                            UIManager.current.DisplayMessage("Cannot unload colonists on " + orders[0].point.LocationName + " (unhabitable).");
                    }
                    else
                    {
                        UIManager.current.DisplayMessage("No colony on " + orders[0].point.LocationName + " for " + ShipName + " to unload colonists at!");
                    }
                }
                Cycle();

            }
            else
            {
                Cycle(); 
            }
        }

        /// <summary>
        /// Get a string description of this spacecraft.
        /// </summary>
        /// <returns>A 4-5 line description including fuel, condition, ship type, etc.</returns>
        public override string GetInfo()
        {
            return base.GetInfo() + "\n" +
                "Colonists " + ColonistOnBoard + " / " + ColonistCapacity; 
        }
    }


}
