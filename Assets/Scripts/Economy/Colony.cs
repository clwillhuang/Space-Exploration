using UnityEngine;
using Ships; 
using System.Collections.Generic;

namespace Economy
{
    /// <summary>
    /// Class for a player colony.
    /// </summary>
    [System.Serializable]
    public class Colony
    {
        /// <summary>
        /// Are minerals on this colony surveyed?
        /// </summary>
        public bool isSurveyed;

        /// <summary>
        /// Is this colony the player's homeworld?
        /// </summary>
        public readonly bool isHomeworld; 

        /// <summary>
        /// What system is this colony located in?
        /// </summary>
        public readonly int systemLocationIndex;

        /// <summary>
        /// Planet this colony is on.
        /// </summary>
        public Point planet { get; private set; } 

        /// <summary>
        /// Is this planet suitable for a human population? 
        /// </summary>
        public readonly bool isHabitable;

        /// <summary>
        /// The name of the celestial body that this colony is located on.
        /// </summary>
        public readonly string PlanetaryBody; 

        /// <summary>
        /// The minerals present on this planet.
        /// </summary>
        public Minerals PlanetaryResources { get; private set; }

        /// <summary>
        /// Mineral stockpiles on this planet.
        /// </summary>
        public float[] MineralStockpiles { get; private set; }

        /// <summary>
        /// The fuel stockpile on this planet.
        /// </summary>
        public float FuelStockpile { get; private set; }

        /// <summary>
        /// The infrastructure on this planet, organized by building type.
        /// </summary>
        public Dictionary<Buildable.BuildingType, int> Infrastructure { get; private set; }

        /// <summary>
        /// The current building being built. 
        /// </summary>
        private Buildable _currentFactoryProject;

        /// <summary>
        /// The current building being built. 
        /// </summary>
        public Buildable CurrentFactoryProject
        {
            get
            {
                return _currentFactoryProject; 
            }

            private set
            {
                if (_currentFactoryProject != value)
                {
                    _currentFactoryProject = value;
                    if (value != null)
                        RemainingFactoryPoints =_currentFactoryProject.buildPoints; 
                }
            }
        }

        /// <summary>
        /// Build a building.
        /// </summary>
        /// <param name="reference">The index reference of the building to be built, in Player.buildableBuildings.</param>
        public void Build(int reference)
        {
            if (reference < 0 || reference >= StateManager.buildableBuildings.Count)
            {
                Debug.LogError("Colony.cs: Invalid reference " + reference);
                return; 
            }

            Debug.Log("Selected new building"); 

            Buildable building = StateManager.buildableBuildings[reference];

            if (CurrentFactoryProject != null && CurrentFactoryProject.buildingType == building.buildingType)
            {
                return;
            }
            
            if (StateManager.currentSM.currentSession.money < building.money) 
                UIManager.current.DisplayMessage("Insufficient money for " + building.name);
            else if (!Minerals.IsSufficient(MineralStockpiles, building.requiredResources))
                UIManager.current.DisplayMessage("Insufficient resources for " + building.name);
            else 
            {    
                for (int i = 0; i < building.requiredResources.Length; i++)
                    MineralStockpiles[i] -= building.requiredResources[i];
                CurrentFactoryProject = building;
                StateManager.currentSM.currentSession.money -= building.money; 
            }
        }

        /// <summary>
        /// The remaining time required to produce the currently selected building. 
        /// </summary>
        public int RemainingFactoryTime { get
            {
                if (GetFactoryProduction == 0f || RemainingFactoryPoints / GetFactoryProduction > 999f)
                    return -1; 
                return Mathf.CeilToInt(RemainingFactoryPoints / GetFactoryProduction); 
            }
        }

        /// <summary>
        /// The remaining build points required to build the current building.
        /// </summary>
        public float RemainingFactoryPoints { get; private set; }

        /// <summary>
        /// The current ship type being built.
        /// </summary>
        private Spaceship.ShipType _currentShipyardProject;

        /// <summary>
        /// The current ship type being built.
        /// </summary>
        public Spaceship.ShipType CurrentShipyardProject
        {
            get
            {
                return _currentShipyardProject;
            }

            set
            {
                if (_currentShipyardProject != value)
                {
                    if (value == Spaceship.ShipType.Null) _currentShipyardProject = value; 
                    else if (Minerals.IsSufficient(MineralStockpiles, Spaceship.MineralCosts[value]))
                    {
                        _currentShipyardProject = value;
                        RemainingShipyardPoints = StateManager.currentSM.currentSession.BuildCostPerShip;
                        for (int i = 0; i < Spaceship.MineralCosts[value].Length; i++)
                            MineralStockpiles[i] -= Spaceship.MineralCosts[value][i];
                    }
                    else
                    {
                        UIManager.current.DisplayMessage("Insufficient resources! Costs: " + Minerals.Resources(Spaceship.MineralCosts[value]));
                    }
                }
            }
        }

        /// <summary>
        /// The remaining time required to produce the currently selected ship to be built. 
        /// </summary>
        public int RemainingShipyardTime
        {
            get
            {
                if (GetShipyardProduction == 0f || RemainingShipyardPoints / GetShipyardProduction > 999f)
                    return -1;
                return Mathf.CeilToInt(RemainingShipyardPoints / GetShipyardProduction);
            }
        }

        public float RemainingShipyardPoints { get; private set; }

        /// <summary>
        /// Updates every day
        /// </summary>
        public void DailyUpdate()
        {
            // Population Growth
            if (Population > 0)
                Population = (Population * (decimal)Constants.DAILY_POP_GROWTH) + 1;
            
            // Factory Production
            if (CurrentFactoryProject != null)
            {
                RemainingFactoryPoints -= GetFactoryProduction;
                if (RemainingFactoryPoints <= 0f)
                {
                    Debug.Log("Finished construction " + CurrentFactoryProject.buildingType);
                    UIManager.current.DisplayMessage(CurrentFactoryProject.name + " was constructed on " + PlanetaryBody);

                    Infrastructure[CurrentFactoryProject.buildingType]++;

                    RemainingFactoryPoints = 0f; 
                    CurrentFactoryProject = null;
                }
            }

            // Ship Production
            if (CurrentShipyardProject != Spaceship.ShipType.Null)
            {
                RemainingShipyardPoints -= GetShipyardProduction;
                if (RemainingShipyardPoints <= 0f)
                {
                    Debug.Log("Finished construction " + CurrentShipyardProject);
                    if (CurrentShipyardProject == Spaceship.ShipType.Cargo) {
                        StateManager.currentSM.currentSession.spaceships.Add(new CargoShip(systemLocationIndex, planet));
                        UIManager.current.DisplayMessage("Cargo ship was constructed on " + PlanetaryBody);
                    }
                    else if (CurrentShipyardProject == Spaceship.ShipType.Colony)
                    {
                        StateManager.currentSM.currentSession.spaceships.Add(new ColonyShip(systemLocationIndex, planet));
                        UIManager.current.DisplayMessage("Colony ship was constructed on " + PlanetaryBody);
                    }
                    else if (CurrentShipyardProject == Spaceship.ShipType.Tanker)
                    {
                        StateManager.currentSM.currentSession.spaceships.Add(new TankerShip(systemLocationIndex, planet));
                        UIManager.current.DisplayMessage("Tanker ship was constructed on " + PlanetaryBody);
                    }
                    else if (CurrentShipyardProject == Spaceship.ShipType.Survey)
                    {
                        StateManager.currentSM.currentSession.spaceships.Add(new SurveyShip(systemLocationIndex, planet));
                        UIManager.current.DisplayMessage("Survey ship was constructed on " + PlanetaryBody);
                    }
                    RemainingShipyardPoints = 0f; 
                    CurrentShipyardProject = Spaceship.ShipType.Null;
                }
            }

            // Mining 
            if (isSurveyed) {
                float[] miningDelta = PlanetaryResources.MineTick(GetMiningProduction);
                for (int i = 0; i < Minerals.MINERALS_NAME.Length; i++)
                    MineralStockpiles[i] += miningDelta[i];
            }

            // Fuel Refinery
         
            if (isRefineryActive)
            {
                int operatingRefineries = Mathf.Min((int)MineralStockpiles[5], Infrastructure[Buildable.BuildingType.Fuel_Refinery]);
                MineralStockpiles[5] -= operatingRefineries;
                FuelStockpile += StateManager.currentSM.currentSession.RefiningEfficiency *
                    operatingRefineries;
            }
            else
                Debug.Log("Refineries inactive");
            
            StateManager.currentSM.currentSession.money += Infrastructure[Buildable.BuildingType.Financial_Centre] * StateManager.currentSM.currentSession.MoneyPerFinancialCenter;

            for (int i = 0; i < Buildable.NumOfBuildings; i++)
                StateManager.currentSM.currentSession.money -= Infrastructure[(Buildable.BuildingType)(i)] * (int)Constants.PER_BUILDING_COST; 
        }

        /// <summary>
        /// The number of human inhabitants on this colony.
        /// </summary>
        public decimal Population { get; private set; }

        /// <summary>
        /// The required population for 100% production efficiency from industries / buildings.
        /// </summary>
        public decimal RequiredPopulation
        {
            get
            {
                decimal r = 0;
                foreach (Buildable buildable in StateManager.buildableBuildings)
                {
                    r += Infrastructure[buildable.buildingType] * buildable.workerPopulation;
                }
                return r;
            }
        }

        /// <summary>
        /// The happiness of the local population, as determined by pop size and leisure facilities present.
        /// </summary>
        public float Happiness {
            get
            {
                return (float)(-decimal.Floor(Population / (decimal)Constants.UNHAPPINESS) + Infrastructure[Buildable.BuildingType.Leisure]);  
            }
        }
        
        /// <summary>
        /// The current operation state of the refineries. True = refineries active, producing fuel.
        /// </summary>
        public bool isRefineryActive { get; set; }

        /// <summary>
        /// Production efficiency determined by the population and required population for present buildings.
        /// </summary>
        /// <returns></returns>
        public float PopulationEfficiency
        {
            get
            {
                if (RequiredPopulation == 0) return 1f;
                if (Population >= RequiredPopulation)
                    return (Happiness > 0f ? 1f : (1f - Mathf.Clamp(Happiness, Constants.UNHAPPINESS_PENALTY, 0f) / Constants.UNHAPPINESS_PENALTY)); 
                else
                {
                    float h = (float)(Population / RequiredPopulation) *
                        (Happiness > 0f ? 1f : (1f - Mathf.Clamp(Happiness, Constants.UNHAPPINESS_PENALTY, 0f) / Constants.UNHAPPINESS_PENALTY));
                    return Mathf.Clamp(h, 0.01f, 1f);
                }
            }
        }

        /// <summary>
        /// The maximum tonnage mined per resource per day. 
        /// </summary>
        /// <returns>Mining production, in tons.</returns>
        public float GetMiningProduction
        {
            get
            {
                return PopulationEfficiency * Infrastructure[Buildable.BuildingType.Mine] * StateManager.currentSM.currentSession.MiningPerMine +
                   Infrastructure[Buildable.BuildingType.Automated_Mine] * StateManager.currentSM.currentSession.MiningPerMine;
            }
        }

        /// <summary>
        /// The daily build points from factories. 
        /// </summary>
        /// <returns>Build points per day.</returns>
        public float GetFactoryProduction
        {
            get
            {
                return PopulationEfficiency * Infrastructure[Buildable.BuildingType.Factory] * StateManager.currentSM.currentSession.ProductionSpeedPerFactory;
            }
        }

        /// <summary>
        /// The daily build points from shipyards.
        /// </summary>
        /// <returns>Build points per day.</returns>
        public float GetShipyardProduction
        {
            get
            {
                return PopulationEfficiency * Infrastructure[Buildable.BuildingType.Shipyard] * StateManager.currentSM.currentSession.ProductionSpeedPerShipyard;
            }
        }

        /// <summary>
        /// Research points by this colony per day from this colony. 
        /// </summary>
        /// <returns>Research points.</returns>
        public float GetResearch
        {
            get
            {
                return PopulationEfficiency * Infrastructure[Buildable.BuildingType.Research_Lab] * StateManager.currentSM.currentSession.ResearchSpeedPerResearchLab;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_planetaryBody">The name of the planet this colony is on.</param>
        /// <param name="minerals">The minerals of the planet.</param>
        /// <param name="_isHabitable">Whether or not this planet is suitable for human population.</param>
        /// <param name="_isSurveyed">Whether or not this planet's minerals has been surveyed.</param>
        public Colony(Point _planet, string _planetaryBody, Minerals minerals, bool _isHabitable, bool _isSurveyed, int _systemLocationIndex, bool _isHomeworld = false)
        {
            planet = _planet;
            PlanetaryBody = _planetaryBody;
            Population = _isHomeworld ? Constants.HOMEWORLD_STARTING_POPULATION : 0;
            isHabitable = _isHabitable;
            PlanetaryResources = minerals;
            MineralStockpiles = new float[Minerals.MINERALS_NAME.Length];
            isSurveyed = _isSurveyed || isHomeworld;
            isRefineryActive = true;
            systemLocationIndex = _systemLocationIndex;
            isHomeworld = _isHomeworld; 

            for (int i = 0; i < Minerals.MINERALS_NAME.Length; i++)
                MineralStockpiles[i] = 0f; 

            Infrastructure = new Dictionary<Buildable.BuildingType, int>();

            for (int i = 0; i < Buildable.NumOfBuildings; i++)
                Infrastructure[(Buildable.BuildingType)i] = 0; 

            if (_isHomeworld)
            {
                Infrastructure[Buildable.BuildingType.Automated_Mine] = 20;
                Infrastructure[Buildable.BuildingType.Factory] = 10;
                Infrastructure[Buildable.BuildingType.Financial_Centre] = 10;
                Infrastructure[Buildable.BuildingType.Fuel_Refinery] = 10;
                Infrastructure[Buildable.BuildingType.Leisure] = 40;
                Infrastructure[Buildable.BuildingType.Mine] = 100;
                Infrastructure[Buildable.BuildingType.Research_Lab] = 10;
                Infrastructure[Buildable.BuildingType.Shipyard] = 5;

                for (int i = 0; i < Minerals.MINERALS_NAME.Length; i++)
                    MineralStockpiles[i] = 80000f;
            }

        }

        #region Ship Functionality

        /// <summary>
        /// Return the fuel to refuel ship with. 
        /// </summary>
        /// <param name="maxFuelToLoad">Max amount of fuel ship can be given according to its capacity and current fuel level.</param>
        /// <returns>The number of litres of fuel to be moved from colony to ship.</returns>
        public float Refuel(float maxFuelToLoad)
        {
            float fuel = Mathf.Min(maxFuelToLoad, FuelStockpile);
            FuelStockpile -= fuel;
            return fuel; 
        }

        /// <summary>
        /// Unload fuel to colony.
        /// </summary>
        /// <param name="fuel">Fuel to be unloaded, in litres.</param>
        public void UnloadFuel(float fuel)
        {
            FuelStockpile += fuel;
        }

        /// <summary>
        /// Remove colonists from colony to load to a ship. 
        /// </summary>
        /// <param name="maxColonistsToLoad">The max # of colonists that can be loaded to ship.</param>
        /// <returns>The # of colonists to be transferred to ship.</returns>
        public int LoadColonists(int maxColonistsToLoad)
        {
            int colonists = 0;
            if ((decimal)maxColonistsToLoad < Population)
                colonists = (int)maxColonistsToLoad;
            else colonists = (int)Population;
            Population -= (int)(colonists);
            return colonists;
        }

        /// <summary>
        /// Unload colonists to colony.
        /// </summary>
        /// <param name="colonists"># of Colonists to be unloaded.</param>
        public int UnloadColonists(int colonists)
        {
            Population += colonists;
            return colonists;
        }

        /// <summary>
        /// Load a mineral from the colony to a ship. 
        /// </summary>
        /// <param name="delta">The max amount of the mineral to be loaded.</param>
        /// <param name="mineralIndex">The type of mineral being loaded.</param>
        /// <returns>The amount, in tons, of mineral being loaded to ship.</returns>
        public float LoadMineralsToShip(float delta, int mineralIndex)
        {
            float minerals = Mathf.Min(MineralStockpiles[mineralIndex], delta);
            MineralStockpiles[mineralIndex] -= minerals;
            return minerals;
        }

        /// <summary>
        /// Unload a ship's mineral cargo to the colony.
        /// </summary>
        /// <param name="unloadedMinerals">All minerals to be unloaded.</param>
        public void UnloadAllMinerals(float[] unloadedMinerals)
        {
            for (int i = 0; i < unloadedMinerals.Length; i++)
                MineralStockpiles[i] += unloadedMinerals[i]; 
            return;
        }

        /// <summary>
        /// Load/unload a building from/to the colony.
        /// </summary>
        /// <param name="type">The type of building to be loaded/unloaded.</param>
        /// <param name="drop">True if colony is being unloaded to colony, false otherwise.</param>
        public void ChangeBuilding(Buildable.BuildingType type, bool drop)
        {
            foreach (Buildable buildable in StateManager.buildableBuildings)
            {
                if (buildable.buildingType == type)
                {
                    if (drop)
                    {
                        Infrastructure[(type)]++;
                    }
                    else
                    {
                        Infrastructure[ (type)]--;
                    }
                    break;
                }
            }
        }

        #endregion 
    }


}
