using System.Collections.Generic;
using Economy;
using Space;
using UnityEngine;

namespace Ships
{
    /// <summary>
    /// A cargo ship.
    /// </summary>
    [System.Serializable]
    public class CargoShip : Spaceship
    {
        /// <summary>
        /// The minerals loaded on this ship.
        /// </summary>
        private float[] minerals { get; set; }   
        
        /// <summary>
        /// The index of the current building type that is loaded onto this ship. 
        /// </summary>
        private int loadedBuilding;

        /// <summary>
        /// The current amount of cargo on this ship. 
        /// </summary>
        public int CargoPoints {
            get
            {
                if (loadedBuilding != -1)
                {
                    return CargoCapacity; 
                }
                else
                {
                    int sum = 0;
                    for (int i = 0; i < minerals.Length; i++)
                    {
                        sum += (int)Mathf.Ceil(minerals[i]); 
                    }
                    return sum; 
                }
            }

        }

        /// <summary>
        /// The max cargo capacity of this ship. 1 mineral = 1 pt.
        /// </summary>
        public int CargoCapacity { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_systemLocationIndex">Index of system this ship was built in.</param>
        /// <param name="_initialOrbitLocation">The initial location of this ship.</param>
        public CargoShip(int _systemLocationIndex, Point _initialOrbitLocation) : 
            base(ShipType.Cargo, "Cargo Ship (C-" + StateManager.currentSM.currentSession.NumberOfShipsEverBuilt + ")", _systemLocationIndex, _initialOrbitLocation)
        {
            systemLocationIndex = _systemLocationIndex;
            loadedBuilding = -1;
            minerals = new float[Minerals.MINERALS_NAME.Length];
            CargoCapacity = 100000;
        }

        /// <summary>
        /// The possible orders for a cargo ship. Not implemented.
        /// </summary>
        public override string[] OrderTypes
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        protected override void Awake() {
            
            base.Awake();

            minerals = new float[Minerals.MINERALS_NAME.Length]; 
            for (int i = 0; i < Minerals.MINERALS_NAME.Length; i++)
                minerals[i] = 0; 
                
            loadedBuilding = -1; 
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
            else if (orderType >= 2 && orders[index].orderType < 2 + Buildable.NumOfBuildings)
            {
                return "Load a " + Buildable.BUILDING_NAMES[orderType - 2] + " at " + place;
            }
            else if (orderType == 2 + Buildable.NumOfBuildings)
            {
                return "Unload building" + " at " + place;
            }
            else if (orderType >= 2 + Buildable.NumOfBuildings + 1 && orderType < 2 + Buildable.NumOfBuildings + 1 + Minerals.NumMinerals)
                return "Load a mineral : " + Minerals.MINERALS_NAME[orderType - 2 - Buildable.NumOfBuildings - 1] + " at " + place;
            else if (orderType == 2 + Buildable.NumOfBuildings + 1 + Minerals.NumMinerals)
                return "Unload all minerals at " + place;
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

            else
            {
                int index = orders[0].orderType;

                // check if colony exists
                if (!(orders[0].point is OrbitingBody) || (orders[0].point as OrbitingBody).colony == null)
                {
                    UIManager.current.DisplayMessage("No colony on " + orders[0].point.LocationName + " for " + ShipName + " to load/unload at!");
                    Cycle();
                    return;
                }
                Colony c = (orders[0].point as OrbitingBody).colony; 

                // load a building
                if (index >= 2 && index < 2 + Buildable.NumOfBuildings)
                {
                    Debug.Log("Index: " + index + " " + "Load " + Buildable.BUILDING_NAMES[index - 2]);

                    if (loadedBuilding == -1 && CargoPoints < CargoCapacity)
                    {
                        Buildable.BuildingType buildingType = (Buildable.BuildingType)(index - 2);
                        if (c.Infrastructure[buildingType] > 0)
                        {
                            c.ChangeBuilding(buildingType, false);
                            loadedBuilding = index - 2;
                        }
                        Cycle();
                    }
                    else
                        Cycle();
                }
                // unload a building
                else if (index == 2 + Buildable.NumOfBuildings)
                {
                    Debug.Log("Index: " + index + " " + "Unload Building");
                    c.ChangeBuilding((Buildable.BuildingType)(loadedBuilding), true);
                    loadedBuilding = -1;
                    Cycle();
                }
                // load resources
                else if (index >= 2 + Buildable.NumOfBuildings + 1 && index < 2 + Buildable.NumOfBuildings + 1 + Minerals.NumMinerals)
                {
                    int minIndex = index - 2 - 1 - Buildable.NumOfBuildings;
                    Debug.Log("Index: " + index + " " + "Load mineral " + Minerals.MINERALS_NAME[minIndex]);
                    minerals[minIndex] += c.LoadMineralsToShip(Mathf.Max(0f, CargoCapacity - CargoPoints), minIndex);
                    Cycle(); 
                }
                // unload all minerals
                else if (index == 2 + Buildable.NumOfBuildings + 1 + Minerals.NumMinerals)
                {
                    Debug.Log("Index: " + index + " " + "Unload all minerals.");
                    c.UnloadAllMinerals(minerals);
                    for (int i = 0; i < Minerals.MINERALS_NAME.Length; i++)
                        minerals[i] = 0f;
                    Cycle(); 
                }
                else
                {
                    Cycle(); 
                }
            }
        }

        /// <summary>
        /// Get a string description of this spacecraft.
        /// </summary>
        /// <returns>A 4-5 line description including fuel, condition, ship type, etc.</returns>
        public override string GetInfo()
        {
            if (loadedBuilding != -1)
            {
                return base.GetInfo() + "\n" +
                    "Building Aboard: " + (Buildable.BuildingType)(loadedBuilding) + "\nCapacity: 1 Building or " + CargoCapacity + " tons of minerals.";
            }
            else if (Mathf.RoundToInt(CargoPoints * 100) / 100 == 0f)
            {
                return base.GetInfo() + "\n" +
                      "Nothing loaded.\nCapacity: 1 Building or " + CargoCapacity + " tons of minerals.";
            }
            else
            {
                string r = "[";

                for (int i = 0; i < minerals.Length; i++)
                {
                    r += minerals[i];
                    r += (i == Minerals.MINERALS_NAME.Length - 1 ? "]" : ", ");
                }

                return base.GetInfo() + "\n" +
                    "Minerals loaded: " + r + "\nCapacity: 1 Building or " + CargoCapacity + " tons of minerals."; 
            }
        }
    }
}
