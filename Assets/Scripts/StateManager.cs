using UnityEngine;
using Economy;
using Ships;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The master controlling class for the game.
/// </summary>
public class StateManager : MonoBehaviour {

    /// <summary>
    /// All possible technologies that a player can research.
    /// </summary>
    public static List<Technology> availableTechnologies;

    /// <summary>
    /// All possible buildings that a player can build.
    /// </summary>
    public static List<Buildable> buildableBuildings;

    /// <summary>
    /// The current player session / save being played.
    /// </summary>
    public PlayerSession currentSession; 

    /// <summary>
    /// Current instance.
    /// </summary>
    private static StateManager _currentSM;

    /// <summary>
    /// Retrieve current instance.
    /// </summary>
    public static StateManager currentSM {
        get
        {
            if (_currentSM == null)
            {
                _currentSM = FindObjectOfType<StateManager>();
            }
            return _currentSM; 
        }    
    }

    /// <summary>
    /// When program is loading, load in technologies and buildings.
    /// </summary>
    public void Awake()
    {
        availableTechnologies = new List<Technology>();

        // Funds
        availableTechnologies.Add(new Technology(
            "Space Exploration Grant I",
            "Appeal to homeworld governments for space program funding.", 
            100f, 
            "", 
            Technology.MoneyGrant,
            1000000f));
        availableTechnologies.Add(new Technology(
            "Space Exploration Grant II",
            "Launch an effort to raise investment in the space exploration and resource mining business.",
            10000f, 
            "", 
            Technology.MoneyGrant,
            1000000f));
        availableTechnologies.Add(new Technology(
            "Space Exploration Grant III",
            "Perform an international funding campaign with large multinationals and investor groups to generate new capital.",
            300000f,
            "",
            Technology.MoneyGrant,
            50000000f));

        // Ship Speed 
        availableTechnologies.Add(new Technology(
            "Spaceship Speed I",
            "Research improved ion drive technology for new spaceship engines. Instantly increases the speed of all ships you own.",
            1000f,
            "",
            Technology.ShipSpeed,
            100f));
        availableTechnologies.Add(new Technology(
            "Spaceship Speed II",
            "Research primitive fusion reactor drive technology for new spaceship engines. Instantly increases the speed of all ships you own.",
            10000f,
            "Spaceship Speed I",
            Technology.ShipSpeed,
            100f));
        availableTechnologies.Add(new Technology(
            "Spaceship Speed III",
            "Research improved fusion reactor drive technology for new spaceship engines. Instantly increases the speed of all ships you own.",
            50000f,
            "Spaceship Speed II",
            Technology.ShipSpeed,
            100f));
        availableTechnologies.Add(new Technology(
            "Spaceship Speed IV",
            "Research advanced fusion reactor drive technology for new spaceship engines. Instantly increases the speed of all ships you own.",
            125000f,
            "Spaceship Speed III",
            Technology.ShipSpeed,
            100f));
        availableTechnologies.Add(new Technology(
            "Spaceship Speed V",
            "Research primitive anti-matter reactor drive technology for new spaceship engines. Instantly increases the speed of all ships you own.",
            300000f,
            "Spaceship Speed IV",
            Technology.ShipSpeed,
            100f));
        availableTechnologies.Add(new Technology(
            "Spaceship Speed VI",
            "Research more efficient and powerful anti-matter engines to increase the speed of all your ships.",
            500000f,
            "Spaceship Speed V",
            Technology.ShipSpeed,
            100f));

        // Mining 
        availableTechnologies.Add(new Technology(
            "Improved Mining I",
            "Improve how your colonies locate suitable minerals for mining.",
            500f,
            "",
            Technology.MiningSpeed,
            0.5f));
        availableTechnologies.Add(new Technology(
            "Improved Mining II",
            "Dramatically increase the rate of mining by implementing use of comuter-aided mining elements.",
            25000f,
            "Improved Mining I",
            Technology.MiningSpeed,
            0.5f));
        availableTechnologies.Add(new Technology(
            "Improved Mining III",
            "Automate critical stages of the mining sector to improve efficiency and mineral yield.",
            250000f,
            "Improved Mining II",
            Technology.MiningSpeed,
            0.5f));
        availableTechnologies.Add(new Technology(
            "Improved Mining IV",
            "Implement breakthrough research in improving resource location and mineral refinement.",
            750000f,
            "Improved Mining III",
            Technology.MiningSpeed,
            1f));
        availableTechnologies.Add(new Technology(
            "Improved Mining V",
            "Research improved mining methods to increase yield and efficiency.",
            1000000f,
            "Improved Mining IV",
            Technology.MiningSpeed,
            1f));

        // Refining
        availableTechnologies.Add(new Technology(
            "Refining Efficiency I",
            "Implement advanced fuel refining practices reducing waste and increasing field yield per Sorium per refinery.",
            1500f,
            "",
            Technology.RefinerySpeed,
            4000f));
        availableTechnologies.Add(new Technology(
            "Refining Efficiency II",
            "By making breakthroughs in sorium chemistry, the fuel production rate of refineries can be increased.",
            250000f,
            "Refining Efficiency I",
            Technology.RefinerySpeed,
            5000f));
        availableTechnologies.Add(new Technology(
            "Refining Efficiency III",
            "Implement newly found methods of sorium refinement which are quicker and more efficient to dramatically improve the fuel yield of your refineries.",
            500000f,
            "Refining Efficiency II",
            Technology.RefinerySpeed,
            5000f));

        // Survey
        availableTechnologies.Add(new Technology(
            "Survey Speed I",
            "Research improved precision gravitational and geological sensors, using updated knowledge of planetary geology and electromagnetism, to improve speed of surveying planets and possible jump point locations.",
            50000f,
            "",
            Technology.SurveySpeed,
            25f));
        availableTechnologies.Add(new Technology(
            "Survey Speed II",
            "Research advanced precision gravitational and geological sensors to improve speed of surveying planets and possible jump point locations.",
            100000f,
            "Survey Speed I",
            Technology.SurveySpeed,
            25f));
        availableTechnologies.Add(new Technology(
            "Survey Speed III",
            "Develop precision gravitational and geological sensors utilizing advanced quantum mechanics to improve speed of surveying planets and possible jump point locations.",
            250000f,
            "Survey Speed II",
            Technology.SurveySpeed,
            25f));

        // Research
        availableTechnologies.Add(new Technology(
            "Research Speed I",
            "Restructure your research facilities to increase efficiency and focus on relevant space technologies.",
            100000f,
            "",
            Technology.ResearchSpeed,
            10f));
        availableTechnologies.Add(new Technology(
            "Research Speed II",
            "Implement the latest advancements in machine-learning and self-learning artificial intelligence to greatly expand the capability of your research facilities.",
            500000f,
            "Research Speed I",
            Technology.ResearchSpeed,
            10f));
        availableTechnologies.Add(new Technology(
            "Research Speed III",
            "Fund research projects to be conducted in low-planet orbit around colonies, to provide greater insight on astrophysics and propulsion.",
            750000f,
            "Research Speed II",
            Technology.ResearchSpeed,
            10f));
        availableTechnologies.Add(new Technology(
             "Research Speed IV",
             "Expand the budget.",
             1000000f,
             "Research Speed III",
             Technology.ResearchSpeed,
             10f));

        // Factory Production
        availableTechnologies.Add(new Technology(
            "Factory Production I",
            "Improve methods of mineral transportation, so that factories produce goods at a faster rate due to reliable mineral shipments.",
            50000f,
            "",
            Technology.FactoryProduction,
            5f));
        availableTechnologies.Add(new Technology(
            "Factory Production II",
            "Restructure factories for greater efficiency due to updated knowledge of productivity.",
            750000f,
            "Factory Production I",
            Technology.FactoryProduction,
            10f));
        availableTechnologies.Add(new Technology(
            "Factory Production III",
            "",
            1000000f,
            "Factory Production II",
            Technology.FactoryProduction,
            15f));
        availableTechnologies.Add(new Technology(
            "Factory Production IV",
            "Streamline industrial production processes to create fluid assembly lines and reduced times.",
            1500000f,
            "Factory Production III",
            Technology.FactoryProduction,
            15f));

        // Shipyard Production
        availableTechnologies.Add(new Technology(
            "Shipyard Production I",
            "Reorganize the organization of shipyards to improve shipbuilding efficiency.",
            50000f,
            "",
            Technology.ShipyardProduction,
            5f));
        availableTechnologies.Add(new Technology(
            "Shipyard Production II",
            "Improve the coordination of shipyard complexes on your colonies to produce ships faster.",
            250000f,
            "Shipyard Production I",
            Technology.ShipyardProduction,
            10f));
        availableTechnologies.Add(new Technology(
            "Shipyard Production III",
            "Streamline the shipbuilding production line to produce ships faster.",
            1000000f,
            "Shipyard Production II",
            Technology.ShipyardProduction,
            10f));
        availableTechnologies.Add(new Technology(
            "Shipyard Production IV",
            "Establish supply chains to prefabricate ship components separately, increasing rate of shipyard production.",
            2500000f,
            "Shipyard Production III",
            Technology.ShipyardProduction,
            15f));
        availableTechnologies.Add(new Technology(
            "Shipyard Production V",
            "Improve shipyard supply chains through better transportation and logistics management.",
            50000000f,
            "Shipyard Production IV",
            Technology.ShipyardProduction,
            15f));

        // Economy 
        availableTechnologies.Add(new Technology(
            "Civilian Economy Expansion I",
            "Promote investment in the industrial sector for greater emphasis on space expansion.",
            80000f,
            "",
            Technology.Economy,
            2));
        availableTechnologies.Add(new Technology(
            "Civilian Economy Expansion II",
            "Look into policies to promote economic growth within the mining and fuel refining businesses.",
            250000f,
            "Civilian Economy Expansion I",
            Technology.Economy,
            2));
        availableTechnologies.Add(new Technology(
            "Civilian Economy Expansion III",
            "Implement new ideas in economic policy to encourage economic growth in newly created fields pertaining to the spaceship building business.",
            750000f,
            "Civilian Economy Expansion II",
            Technology.Economy,
            3));
        availableTechnologies.Add(new Technology(
            "Civilian Economy Expansion IV",
            "Establish new industries such as asteroid mining and space tourism.",
            1500000f,
            "Civilian Economy Expansion III",
            Technology.Economy,
            3));
        availableTechnologies.Add(new Technology(
            "Civilian Economy Expansion V",
            "Facilitate trade between colonies on different planets and in different star systems, to drive economic growth and profit.",
            3000000f,
            "Civilian Economy Expansion IV",
            Technology.Economy,
            4));

        buildableBuildings = new List<Buildable>()
        {
            new Buildable(Buildable.BuildingType.Automated_Mine,
            0,
            "Automated Mine",
            100000,
            20000f,
            new float[8] { 1000f, 0f, 0f, 3000f, 0f, 0f, 0f, 0f }),

            new Buildable(Buildable.BuildingType.Factory,
            125000,
            "Factory",
            100000,
            35000f,
            new float[8] { 10000f, 100f, 2500f, 0f, 750f, 0f, 0f, 0f }),

            new Buildable(Buildable.BuildingType.Fuel_Refinery,
            80000,
            "Fuel Refinery",
            100000,
            30000f,
            new float[8] { 1000f, 0f, 0f, 0f, 750f, 750f, 0f, 750f }),

            new Buildable(Buildable.BuildingType.Leisure,
            60000,
            "Leisure",
            100000,
            5000f,
            new float[8] { 1000f, 0f, 0f, 0f, 0f, 0f, 0f, 1500f }),

            new Buildable(Buildable.BuildingType.Mine,
            75000,
            "Mine",
            100000,
            15000f,
            new float[8] { 2500f, 0f, 0f, 1000f, 0f, 0f, 0f, 0f }),

            new Buildable(Buildable.BuildingType.Research_Lab,
            150000,
            "Research Lab",
            200000,
            60000f,
            new float[8] { 10000f, 0f, 0f, 0f, 5000f, 0f, 0f, 0f }),

            new Buildable(Buildable.BuildingType.Shipyard,
            125000,
            "Shipyard",
            100000,
            50000f,
            new float[8] { 1000f, 5000f, 0f, 0f, 0f, 200f, 0f, 0f }),

            new Buildable(Buildable.BuildingType.Financial_Centre,
            50000,
            "Financial Centre",
            1000,
            10000f,
            new float[8] { 2000f, 250f, 0f, 0f, 0f, 200f, 0f, 0f }),
        };
    }

    /// <summary>
    /// Begin the clock and initiate a new player session.
    /// </summary>
    public void Start()
    {
        currentSession = new PlayerSession();

        StartCoroutine(Clock());
    }

    /// <summary>
    /// Update every frame. 
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            UIManager.current.UpdateSystem();
        }
        if (currentSession != null && currentSession.TimeMode != 0)
        {
            currentSession.Update();
            // Some developer commands that may as well be kept for demo.
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                currentSession.spaceships.Add(new CargoShip(currentSession.colonies[0].systemLocationIndex, currentSession.colonies[0].planet));
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                currentSession.spaceships.Add(new ColonyShip(currentSession.colonies[0].systemLocationIndex, currentSession.colonies[0].planet));

            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                currentSession.spaceships.Add(new SurveyShip(currentSession.colonies[0].systemLocationIndex, currentSession.colonies[0].planet));

            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                currentSession.spaceships.Add(new TankerShip(currentSession.colonies[0].systemLocationIndex, currentSession.colonies[0].planet));

            }
        }
     
    }

    /// <summary>
    /// Method called by UI when building is picked.
    /// </summary>
    /// <param name="reference"></param>
    public void SelectBuilding(int reference)
    {
        currentSession.colonies[currentSession.SelectedColony].Build(reference);
        UIManager.current.buildingSelector.SetActive(false);
    }

    /// <summary>
    /// Time keeping.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Clock()
    {
        while (true)
        {
            if (currentSession.TimeMode == 0)
            {
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                yield return new WaitForSeconds(Constants.SECONDS_PER_HOUR / currentSession.TimeMode);
               // Debug.Log("New Hour!");

                currentSession.HourlyUpdate(); 
            }
        }
    }
}
