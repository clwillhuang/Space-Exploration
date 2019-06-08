using System.Collections.Generic; 
using UnityEngine;
using Space; 

namespace Ships
{
    /// <summary>
    /// Base class for a spaceship.
    /// </summary>
    [System.Serializable]
    public abstract class Spaceship
    {
        /// <summary>
        /// The index of the system that this ship is currently in.
        /// </summary>
        public int systemLocationIndex;

        public bool flaggedForDestruction = false;

        public static Dictionary<ShipType, float[]> MineralCosts = new Dictionary<ShipType, float[]>
        {
            { ShipType.Cargo, new float[8] { 20000f, 100f, 2500f, 0f, 750f, 120f, 6000f, 0f } },
            { ShipType.Colony, new float[8] { 20000f, 100f, 2500f, 0f, 8000f, 0f, 5000f, 0f } },
            { ShipType.Survey, new float[8] { 20000f, 100f, 2500f, 0f, 750f, 0f, 3000f, 3000f } },
            { ShipType.Tanker, new float[8] { 20000f, 100f, 2500f, 0f, 750f, 240f, 5000f, 0f } },
        }; 

        /// <summary>
        /// The current order last in the list of orders for this ship. 
        /// </summary>
        private Order lastOrder
        {
            get
            {
                if (orders.Count == 0)
                    return null;
                else
                    return orders[orders.Count - 1];
            }
        }

        /// <summary>
        /// Return the index of the system that the ship will visit last according to its current orders.
        /// </summary>
        public int lastSystemLocationIndex
        {
            get
            {
                if (orders.Count == 0) return systemLocationIndex;
                else
                {
                    if (lastOrder.point is JumpPoint)
                    {
                        JumpPoint jp = lastOrder.point as JumpPoint;
                        if (jp.IsValidConnection() && jp.isSurveyed && lastOrder.orderType == 0)
                            return jp.connectingSystem;
                        else
                            return lastOrder.systemIndex;
                    }
                    return lastOrder.systemIndex;

                }
            }
        }

        public Vector2 localPositionInSystem { get; private set; }

        /// <summary>
        /// The fuel tank capacity of this ship, in litres. 
        /// </summary>
        public float FuelCapacity { get; protected set; }

        /// <summary>
        /// The current amount of fuel in the tank, in litres.
        /// </summary>
        public float FuelOnBoard { get; protected set; }

        /// <summary>
        /// The current repair condition of the spaceship. (0f - 100f)
        /// </summary>
        public float Condition { get; protected set; }

        /// <summary>
        /// The name of this ship. 
        /// </summary>
        public string ShipName { get; protected set; }

        /// <summary>
        /// The ships speed in space.
        /// </summary>
        public float Speed
        {
            get
            {
                return StateManager.currentSM.currentSession.SpaceshipSpeed;
            }
        }

        /// <summary>
        /// The type of ships allowed + null. 
        /// </summary>
        public enum ShipType { Null = 0, Cargo, Colony, Survey, Tanker }

        /// <summary>
        /// The names of each ship type.
        /// </summary>
        public static readonly string[] ShipTypeNames = { "Nothing", "Cargo Ship", "Colony Ship", "Survey Ship", "Tanker Ship" };

        /// <summary>
        /// This ship's ship type.
        /// </summary>
        public readonly ShipType shiptype; 

        /// <summary>
        /// The names for each possible order type for this ship. Varies per ship type.
        /// </summary>
        public abstract string[] OrderTypes { get; }

        /// <summary>
        /// The orders for this spaceship. (Note: queue not suitable, b/c all orders must be readable)
        /// </summary>
        protected List<Order> orders;

        /// <summary>
        /// The current object that the spacecraft is orbiting around.
        /// </summary>
        public Point orbitingObject { get; protected set; }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="_shiptype">The ship type</param>
        /// <param name="_ShipName">The name of this ship.</param>
        /// <param name="_systemLocationIndex">The index of the system where this ship is to be spawned at.</param>
        /// <param name="_initialOrbitLocation">The point where it is to be spawned at.</param>
        public Spaceship(ShipType _shiptype, string _ShipName, int _systemLocationIndex, Point _initialOrbitLocation) 
        {
            ShipName = _ShipName;
            shiptype = _shiptype;
            orders = new List<Order>();
            systemLocationIndex = _systemLocationIndex;
            orbitingObject = _initialOrbitLocation;
            localPositionInSystem = new Vector2(_initialOrbitLocation.CoordX(), _initialOrbitLocation.CoordY());

            if (_shiptype == ShipType.Tanker)
                FuelCapacity = 10000000f;
            else
                FuelCapacity = 1000000f;

            FuelOnBoard = 0f;
            Condition = 100f;

            if (systemLocationIndex == UIManager.current.CurrentlyDisplayedSystem)
            {
                UIManager.current.ShowShip(this);
            }

            StateManager.currentSM.currentSession.NumberOfShipsEverBuilt++; 
        }
        
        protected virtual void Awake() {
            orders = new List<Order>(); 
        }

        /// <summary>
        /// Add a new order to this ship. 
        /// </summary>
        /// <param name="newOrder">The location of the order.</param>
        /// <param name="type">The type of order.</param>
        /// <param name="systemIndex">The index of the system where the order takes place.</param>
        public virtual void AddOrder(Point newOrder, int type, int systemIndex)
        {
            if (type != 0)
            {
                if (orders.Count == 0)
                    orders.Add(new Order(newOrder, 0, systemIndex));
                else if (orders[orders.Count - 1].point != newOrder)
                    orders.Add(new Order(newOrder, 0, systemIndex));
            }
            orders.Add(new Order(newOrder, type, systemIndex));
        }

        /// <summary>
        /// Get the string representation of an order.
        /// </summary>
        /// <param name="index">The index of the order to be retrieved.</param>
        /// <returns>String representation of the order, complete with type and location.</returns>
        public abstract string GetOrder(int index);

        /// <summary>
        /// Get a list of all the orders this spaceship has.
        /// </summary>
        /// <returns>List of orders, one per line.</returns>
        public string GetAllOrders()
        {
            string r = ""; 
            for (int i = 0; i < orders.Count; i++)
            {
                r += GetOrder(i) + "\n"; 
            }
            return r; 
        }

        public void UpdateConditionAndVerifyOrders()
        {
            // Verify orders
            if (orders.Count > 0 && orders[0] != null)
            {
                if (orders[0].orderType > 0)
                {
                    // Verify survey point is valid
                    if (shiptype == ShipType.Survey && orders[0].orderType == 2)
                    {
                        if (orders[0].point == null) return;
                    }
                    else // Verify orders that require a colony
                    {
                        if (!(orders[0].point is OrbitingBody)) Cycle();
                        else if ((orders[0].point as OrbitingBody).colony == null) Cycle(); 
                    }
                }
            }

            // If at random point in space, degrade spacecraft
            if (orbitingObject == null)
                Condition -= Constants.SHIP_DEGRADATION_PER_SECOND * Time.deltaTime * StateManager.currentSM.currentSession.TimeMode;
            else
            {
                // If not at planet, degrade spacecraft
                if (!(orbitingObject is OrbitingBody)) Condition -= Constants.SHIP_DEGRADATION_PER_SECOND * Time.deltaTime * StateManager.currentSM.currentSession.TimeMode;
                // If there is no colony at the planet, degrade spacecraft
                else if ((orbitingObject as OrbitingBody).colony == null) Condition -= Constants.SHIP_DEGRADATION_PER_SECOND * Time.deltaTime * StateManager.currentSM.currentSession.TimeMode;
                // Else instantly repair at a colony
                else Condition = 100f;
            }
            Condition = Mathf.Max(0f, Condition);
            if (Condition <= 0f)
            {
                Debug.Log("Destroying ships due to condition: " + ShipName);
                UIManager.current.DisplayMessage(ShipName + " condition has degraded to destruction. Ship was abandoned.");
                flaggedForDestruction = true;
            }
        }

        /// <summary>
        /// Process the orders. Once per in-game frame. 
        /// </summary>
        public virtual void OrderUpdate()
        {
            // Update spacecraft condition 

            if (orders.Count == 0)
            {
                if (orbitingObject != null)
                    localPositionInSystem = new Vector2(orbitingObject.CoordX(), orbitingObject.CoordY());
                return; 
            }
            // goto 
            if (orders[0].orderType == 0)
            {
                if (orbitingObject != orders[0].point)
                {
                    float distance = Vector2.Distance(localPositionInSystem, orders[0].pointVector2);

                    if (FuelOnBoard > 0f && Condition > 0f)
                        localPositionInSystem = Vector2.MoveTowards(localPositionInSystem, orders[0].pointVector2, Speed * StateManager.currentSM.currentSession.TimeMode * Time.deltaTime);
                    else
                        localPositionInSystem = Vector2.MoveTowards(localPositionInSystem, orders[0].pointVector2, Speed * StateManager.currentSM.currentSession.TimeMode * Time.deltaTime / Constants.EMPTY_FUEL_PENALTY);

                    if (FuelOnBoard > 0f)
                        FuelOnBoard -= Time.deltaTime * Constants.FUEL_CONSUMPTION_PER_SECOND;
                    if (FuelOnBoard < 0f)
                        FuelOnBoard = 0f;
                }
                if (orbitingObject == orders[0].point || Vector2.Distance(localPositionInSystem, orders[0].pointVector2) < 3f)
                {
                    // will try to jump through as default
                    if (orders[0].point is JumpPoint)
                    {
                        Debug.Log("Spaceship " + ShipName + " is at a jp"); 
                        if (orders[0].point.isSurveyed)
                        {
                            if (!(orders[0].point as JumpPoint).IsValidConnection())
                            {
                                Cycle();
                                return;
                            }
                            JumpPoint other = StateManager.currentSM.currentSession.galaxy[(orders[0].point as JumpPoint).connectingSystem].JumpPoints[(orders[0].point as JumpPoint).connectingJumpPoint];
                            Debug.Log("Travelled from index " + systemLocationIndex + " to " + (orders[0].point as JumpPoint).connectingSystem); 
                            orbitingObject = other;
                            systemLocationIndex = (orders[0].point as JumpPoint).connectingSystem;
                            localPositionInSystem = new Vector2(other.CoordX(), other.CoordY());
                            if (systemLocationIndex == UIManager.current.CurrentlyDisplayedSystem)
                                UIManager.current.ShowShip(this);
                            else
                                UIManager.current.UpdateSystem(); 
                            Cycle(false);
                        }
                        else
                        {
                            if (shiptype != ShipType.Survey)
                                Debug.LogError(ShipName + " tried to go through an unsurveyed jump point.");
                            orbitingObject = orders[0].point;
                            Cycle();
                        }
                    }
                    // orbiting
                    else
                    {
                        orbitingObject = orders[0].point;
                        Cycle();

                    }
                }
                else orbitingObject = null; 
                
            }
            // refuel
            else if (orders[0].orderType == 1)
            {
                if (orbitingObject == null) orbitingObject = orders[0].point;
                if (!(orbitingObject is OrbitingBody))
                {
                    Cycle();
                    return; 
                }
                else
                {
                    float fuelToLoad = FuelCapacity - FuelOnBoard;
                    if ((orbitingObject as OrbitingBody).colony == null)
                        UIManager.current.DisplayMessage("No colony on " + orders[0].point.LocationName + " for " + ShipName + " to refuel at!");
                    else 
                        FuelOnBoard += (orbitingObject as OrbitingBody).colony.Refuel(fuelToLoad);
                    Cycle(); 
                }
            }
            else
            {
                Cycle(); 
            }
        }

        /// <summary>
        /// Delete that last order.
        /// </summary>
        public void DeleteLast()
        {
            if (orders.Count > 0) orders.RemoveAt(orders.Count - 1); 
        }

        /// <summary>
        /// Complete an order.
        /// </summary>
        /// <param name="changeOrbit">Change orbit location.</param>
        public void Cycle(bool changeOrbit = true)
        {
            if (orders.Count > 0)
            {
                if (changeOrbit)
                    orbitingObject = orders[0].point;
                orders.RemoveAt(0);
            }
        }

        /// <summary>
        /// Get a string description of this spacecraft.
        /// </summary>
        /// <returns>A 4-5 line description including fuel, condition, ship type, etc.</returns>
        public virtual string GetInfo()
        {
            return "Name: " + ShipName + "         " + "Ship Type: " + ShipTypeNames[(int)shiptype] + "\n" +
                "Fuel: " + FuelOnBoard.ToString("N0") + " / " + FuelCapacity.ToString("N0") + "         " + "Condition " + Condition.ToString("N1") + "%\n" +
                "System: " + StateManager.currentSM.currentSession.galaxy[systemLocationIndex].SystemName + (orbitingObject != null ? ", " + orbitingObject.LocationName : ""); 
        }

    }
}
