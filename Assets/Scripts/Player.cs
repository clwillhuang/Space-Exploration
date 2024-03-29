﻿using Space;
using Economy;
using Ships;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for a player save / session, containing ships, colonies, discovered systems, etc.
/// </summary>
[System.Serializable]
public class PlayerSession {

    /// <summary>
    /// The current month the player is in. 
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// The current day within the month.
    /// </summary>
    public int Day { get; set; }

    /// <summary>
    /// The current hour within the day.
    /// </summary>
    public int Hour { get; set; }

    /// <summary>
    /// Current time mode. 
    /// </summary>
    public int TimeMode { get; private set; }

    /// <summary>
    /// Current time mode selected.
    /// </summary>
    public void SetTimeMode(int newMode)
    {
        TimeMode = newMode;
    }

    /// <summary>
    /// All generated star systems thus far
    /// </summary>
    [SerializeField]
    public List<StarSystem> galaxy;

    /// <summary>
    /// The list of names for all discovered star systems.
    /// </summary>
    public List<string> systemNames;

    /// <summary>
    /// Player's colonies
    /// </summary>
    [SerializeField]
    public List<Colony> colonies;

    /// <summary>
    /// The index of the current colony selected by player.
    /// </summary>
    public int SelectedColony { get; private set; }

    /// <summary>
    /// The current colony selected by player.
    /// </summary>
    public Colony currentColony
    {
        get
        {
            return colonies[SelectedColony];
        }
    }

    /// <summary>
    /// The list of spaceships the player owns.
    /// </summary>
    [SerializeField]
    public List<Spaceship> spaceships;

    /// <summary>
    /// The total number of ships the player has ever built. Used for naming purposes.
    /// </summary>
    public int NumberOfShipsEverBuilt = 0;

    /// <summary>
    /// The current colony selected by the player in Colony Summary. 
    /// </summary>
    /// <param name="value"></param>
    public void SetSelectedColony(int value)
    {
        if (value < 0 || value >= colonies.Count)
        {
            Debug.LogError("Player.cs: Invalid reference: " + value);
        }
        SelectedColony = value;
    }

    /// <summary>
    /// The name of this player.
    /// </summary>
    public string name;

    /// <summary>
    /// The player's money balance.
    /// </summary>
    public int money;

    /// <summary>
    /// Number of research labs owned by player across all planets.
    /// </summary>
    [System.NonSerialized]
    public int ResearchLabCount; 

    /// <summary>
    /// Research points per research lab
    /// </summary>
    public float ResearchSpeedPerResearchLab = 10f;

    /// <summary>
    /// Production points per shipyard
    /// </summary>
    public float ProductionSpeedPerFactory = 75f;

    /// <summary>
    /// Shipbuilding points per shipyard
    /// </summary>
    public float ProductionSpeedPerShipyard = 75f; 

    /// <summary>
    /// The units of fuel produced per unit of sorium.
    /// </summary>
    public float RefiningEfficiency = 2000f; 

    /// <summary>
    /// Tons of minerals mined per mine per day. 
    /// </summary>
    public float MiningPerMine = 4f;

    /// <summary>
    /// Money generated by each financial center per day
    /// </summary>
    public int MoneyPerFinancialCenter = 5;

    /// <summary>
    /// Build cost per ship.
    /// </summary>
    public float BuildCostPerShip = 10000f;

    /// <summary>
    /// The speed of each spaceship.
    /// </summary>
    public float SpaceshipSpeed { get; set; }

    /// <summary>
    /// The list of names for technologies that the player has researched.
    /// </summary>
    public List<string> researchedTechnologies;

    /// <summary>
    /// The research pt progress in each technology, accessed by index
    /// </summary>
    public Dictionary<int, float> technologyProgress;

    /// <summary>
    /// The index of the project being currently researched.
    /// </summary>
    private int currentResearch; 

    /// <summary>
    /// The index of the project being currently researched.
    /// </summary>
    public int CurrentResearch
    {
        get
        {
            return currentResearch; 
        }
        set
        {
            if (value >= 0 && value < StateManager.availableTechnologies.Count)
            {
                currentResearch = value;
                if (!technologyProgress.ContainsKey(value))
                {
                    technologyProgress.Add(value, StateManager.availableTechnologies[value].cost);
                }
            }
            else
                Debug.LogError("Player.cs.: Invalid research index assigned: " + value); 
        }
    }

    /// <summary>
    /// The survey points created by a survey vessel per day.
    /// </summary>
    public float SurveySpeed { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="_name"></param>
    public PlayerSession(string _name = "Player")
    {
        Month = 1;
        Day = 1;
        
        TimeMode = 0;

        name = _name;
        systemNames = new List<string>();
        colonies = new List<Colony>(); 

        currentResearch = -1; 
        researchedTechnologies = new List<string>();
        technologyProgress = new Dictionary<int, float>(); 
        for (int i = 0; i < StateManager.availableTechnologies.Count; i++)
        {
            technologyProgress.Add(i, StateManager.availableTechnologies[i].cost); 
        }
        researchedTechnologies.Add("");
        ResearchLabCount = Constants.STARTING_RESEARCH_LABS;
        ResearchSpeedPerResearchLab = 20f;

        spaceships = new List<Spaceship>();
        SurveySpeed = 50f;

        galaxy = new List<StarSystem>();
        galaxy.Add(new StarSystem(0));
        UIManager.current.UpdateDropdown(galaxy[0].SystemName);

        SpaceshipSpeed = 20f;

        // Find home world manually here, instead of during system generation to avoid system error.
        foreach (OrbitingBody planet in galaxy[0].star.OrbitingPlanets)
        {
            if (planet.isHabitable)
            {
                colonies.Add(planet.EstablishHomeWorld());
                planet.Survey();  
                break;
            }
        }

        money = 10000000;
    }

    /// <summary>
    /// Generate a new system, connected to an existing system through an existing jp
    /// </summary>
    /// <param name="starsystem">Existing star system that new system is connected to.</param>
    /// <param name="jp">Existing jump point that connects to new system.</param>
    public void GenerateNewSystem(StarSystem starsystem, JumpPoint jp)
    {
        // Generate a new system
        galaxy.Add(new StarSystem(galaxy.Count, starsystem.index, jp.jumpPointIndex));

        jp.connectingSystem = galaxy.Count - 1;

        Debug.Log("Added new system: " + galaxy[galaxy.Count-1].SystemName);

        UIManager.current.UpdateDropdown(galaxy[galaxy.Count - 1].SystemName);

        UIManager.current.UpdateSystem();

        UIManager.current.DisplayMessage("A new system has been discovered: " + galaxy[galaxy.Count - 1].SystemName);
    }

    /// <summary>
    /// Add a colony to the player.
    /// </summary>
    /// <param name="c">Colony to be added.</param>
    public void AddColony(Colony c)
    {
        Debug.Log("Added colony.");

        if (colonies == null)
        {
            colonies = new List<Colony>();
            Debug.Log("Added colon1y.");
        }

        colonies.Add(c);

        Debug.Log(colonies.Count);

        return;
    }

    public void Awake()
    {

    }

    /// <summary>
    /// Planetary motion update.
    /// </summary>
    public void Update()
    {
        foreach (StarSystem system in galaxy)
        {
            foreach (OrbitingBody planet in system.star.OrbitingPlanets)
            {
                planet.Update();
            }
        }
        foreach (Spaceship ship in spaceships)
        {
            if (!ship.flaggedForDestruction)
                ship.OrderUpdate();
        }
        bool updaterequired = false;
        for (int i = 0; i < spaceships.Count;)
        {
            if (spaceships[i] == null || spaceships[i].flaggedForDestruction)
            {
                spaceships.RemoveAt(i);
                updaterequired = true;
            }
            i++;
        }
        if (updaterequired)
        {
            UIManager.current.ShipScreen.OnEnable();
            UIManager.current.UpdateSystem();
        }
    }

    /// <summary>
    /// Call the daily update. 
    /// </summary>
    public void DailyUpdate()
    {
        foreach (Colony colony in colonies)
        {
            colony.DailyUpdate();
        }
        if (CurrentResearch != -1 && !researchedTechnologies.Contains(StateManager.availableTechnologies[CurrentResearch].technologyName))  
        {
            technologyProgress[CurrentResearch] -= ResearchSpeedPerResearchLab * ResearchLabCount;
            if (technologyProgress[CurrentResearch] < 0f)
            {
                researchedTechnologies.Add(StateManager.availableTechnologies[currentResearch].technologyName);
                StateManager.availableTechnologies[currentResearch].Unlock();  
                technologyProgress[currentResearch] = 0f;
                UIManager.current.DisplayMessage("Research for " + StateManager.availableTechnologies[CurrentResearch].technologyName + " completed.");
                currentResearch = -1;
            }
        }
    }

    /// <summary>
    /// Update the clock per hour.
    /// </summary>
    public void HourlyUpdate()
    {
        Hour++;
        if (Hour > 23)
        {
            Hour = 0; 
            Day++;
            if (Day > 30)
            {
                Day = 1;
                Month++;
            }
            DailyUpdate();
        }
    }
}
