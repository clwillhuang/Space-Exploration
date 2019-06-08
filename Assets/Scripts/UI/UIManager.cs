using UnityEngine;
using UnityEngine.UI;
using Space;
using Ships; 
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Master UI element control.
/// </summary>
public class UIManager : MonoBehaviour {

    /// <summary>
    /// Reference to the ship screen. (unused)
    /// </summary>
    [SerializeField]
    public ShipScreen ShipScreen; 

    /// <summary>
    /// Building selector interface.
    /// </summary>
    [SerializeField]
    public GameObject buildingSelector; 

    /// <summary>
    /// Prefab for a player message
    /// </summary>
    [SerializeField]
    private GameObject messagePrefab;

    /// <summary>
    /// Parent gameobject that new message gameobjects are appended to.
    /// </summary>
    [SerializeField]
    private Transform messageParent; 

    #region System Graphics

    /// <summary>
    /// Prefab for celestial objects. 
    /// </summary>
    [SerializeField]
    private GameObject celestialBodyPrefab;

    /// <summary>
    /// Prefab for jump points.
    /// </summary>
    [SerializeField]
    private GameObject jumpPointPrefab;

    /// <summary>
    /// Prefab for ships.
    /// </summary>
    [SerializeField]
    private GameObject shipPrefab;

    /// <summary>
    /// List of possible sprites for moons.
    /// </summary>
    public List<Sprite> moons;

    /// <summary>
    /// List of possible sprites for habitable planets.
    /// </summary>
    public List<Sprite> planetsHabitable;

    /// <summary>
    /// List of possible sprites for uninhabitable planets.
    /// </summary>
    public List<Sprite> planetsUninhabitable;

    /// <summary>
    /// List of possible sprites for stars.
    /// </summary>
    public List<Sprite> stars;

    /// <summary>
    /// Reference to the central star object in the screen.
    /// </summary>
    [SerializeField]
    private SystemBodyGO centralStar;

    #endregion

    /// <summary>
    /// UI Dropdown allowing players to switch viewed system
    /// </summary>
    [SerializeField]
    private Dropdown SystemDropDown;

    /// <summary>
    /// The index of the system being currently displayed.
    /// </summary>
    public int CurrentlyDisplayedSystem
    {
        get
        {
            return SystemDropDown.value; 
        }
    }

    /// <summary>
    /// List of discovered systems in the galaxy
    /// </summary>
    private List<string> systems;

    /// <summary>
    /// Text elements in the top right corner.
    /// </summary>
    [SerializeField]
    private Text dateBar;

    /// <summary>
    /// Text elements in the top right corner.
    /// </summary>
    [SerializeField]
    private Text infobar;

    /// <summary>
    /// UI elements in the top right corner.
    /// </summary>
    [SerializeField]
    private GameObject StartMenu; 

    private static UIManager _current;

    public static UIManager current
    {
        get
        {
            if (_current == null)
            {
                _current = FindObjectOfType<UIManager>();
            }
            return _current;
        }
    }

    /// <summary>
    /// When program starts loading.
    /// </summary>
    public void Awake()
    {
        systems = new List<string>();
    }

    /// <summary>
    /// When program finishes loading.
    /// </summary>
    public void Start()
    {
        StartMenu.SetActive(true);

        StartCoroutine(SystemUpdateLoop()); 
    }

    /// <summary>
    /// Make sure the UI is updated with the current information.
    /// </summary>
    /// <returns>Nothing.</returns>
    private IEnumerator SystemUpdateLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            if (dateBar == null)
            {
                Debug.LogError("Datebar null.");
                continue;
            }
            else if (StateManager.currentSM.currentSession == null)
            {
                Debug.LogError("Current null.");
                continue;
            }

            dateBar.text = StateManager.currentSM.currentSession.name
                + " (" + StateManager.currentSM.currentSession.money.ToString("C0") +
                ") // Month " + StateManager.currentSM.currentSession.Month +
                " Day " + StateManager.currentSM.currentSession.Day + " " +
                StateManager.currentSM.currentSession.Hour + ":00";

            infobar.text = "Player Attributes: \n" +
                "Mining Productivity: " + StateManager.currentSM.currentSession.MiningPerMine.ToString("0.#") + " tons / resource / mine / day.\n" +
                "Research Speed: " + StateManager.currentSM.currentSession.ResearchSpeedPerResearchLab.ToString("0.#") + " / lab / day.\n" +
                "Revenue: " + StateManager.currentSM.currentSession.MoneyPerFinancialCenter.ToString("C0") + " per financial center / day.\n" +
                "Cost per non-financial centre building: " + Constants.PER_BUILDING_COST.ToString("C0") + " litres per building / day\n" +
                "Fuel Production: " + StateManager.currentSM.currentSession.RefiningEfficiency.ToString("N0") + " per refinery / day.\n" +
                "Factory Productivity: " + StateManager.currentSM.currentSession.ProductionSpeedPerFactory.ToString("0.#") + " build pts / day.\n" +
                "Shipyard Productivity: " + StateManager.currentSM.currentSession.ProductionSpeedPerShipyard.ToString("0.#") + " build pts / day.\n" +
                "Spaceship Speed: " + StateManager.currentSM.currentSession.SpaceshipSpeed.ToString("0.#") + " units.\n" +
                "Spaceship Degradation: -" + Constants.SHIP_DEGRADATION_PER_SECOND.ToString("N2") + "% per normal speed second.\n" +
                "Population Growth: x" + Constants.DAILY_POP_GROWTH.ToString("N5") + " per day.\n" +
                "Zero productivity at : " + Constants.UNHAPPINESS_PENALTY.ToString("N1") + " happiness.\n" +
                "1 Unhappiness per " + Constants.UNHAPPINESS + " colonists on colony."; 
        }
    }

    /// <summary>
    /// Add a new system name into the UI system dropdown.
    /// </summary>
    /// <param name="newSystem"></param>
    public void UpdateDropdown(string newSystem)
    {
        systems.Add(newSystem);

        Debug.Log("Added " + newSystem + " to dropdown. Count : " + systems.Count);

        SystemDropDown.ClearOptions();

        SystemDropDown.AddOptions(systems);

        SystemDropDown.value = systems.Count - 1; 
    }
    
    /// <summary>
    /// Change the system being displayed.
    /// </summary>
    /// <param name="reference">Index reference of the system in galaxy. </param>
    public void ShowSystem(int reference)
    {
        SystemDropDown.value = reference;

        UpdateSystem(); 
    }

    /// <summary>
    /// Update the UI to display the currently selected system. 
    /// </summary>
    public void UpdateSystem()
    {
        Debug.Log("Updating system." + CurrentlyDisplayedSystem + " ");

        StarSystem system = StateManager.currentSM.currentSession.galaxy[CurrentlyDisplayedSystem];

        centralStar.systemBody = system.star;

        foreach (Transform child in centralStar.orbiting)
        {
            Destroy(child.gameObject);
        }

        // Show system bodies in this system
        foreach (SystemBody planet in system.star.OrbitingPlanets)
        {
            Transform newSprite = Instantiate(celestialBodyPrefab, centralStar.orbiting).transform;

            newSprite.GetComponent<SystemBodyGO>().systemBody = planet;

            newSprite.gameObject.name = planet.LocationName;

            foreach (SystemBody moon in planet.OrbitingPlanets)
            {
                Transform newMoonSprite = Instantiate(celestialBodyPrefab, newSprite.Find("Orbit")).transform;

                newMoonSprite.GetComponent<SystemBodyGO>().systemBody = moon;

                newMoonSprite.gameObject.name = moon.LocationName;
            }
        }

        // Show jump points in this system
        foreach (JumpPoint point in system.JumpPoints)
        {
            // Do not show point if it has already been surveyed and leads nowhere
            if (point.isSurveyed && !point.IsValidConnection())
                continue; 

            Transform newSprite = Instantiate(jumpPointPrefab, centralStar.orbiting).transform;

            newSprite.GetComponent<JumpPointGO>().jumpPoint = point;

            newSprite.localPosition = new Vector3(point.CoordX(), point.CoordY(), 0f);
        }

        // Show ships in this system
        foreach (Spaceship spaceship in StateManager.currentSM.currentSession.spaceships)
        {
            if (spaceship.systemLocationIndex != CurrentlyDisplayedSystem) continue;
            else
            {
                Transform newShip = Instantiate(shipPrefab, centralStar.orbiting).transform;
                newShip.GetComponent<SpaceShipGO>().spaceship = spaceship;
            }
        }
    }

    /// <summary>
    /// Make sure a ship is being displayed in the currently selected system.
    /// </summary>
    /// <param name="spaceship"></param>
    public void ShowShip(Spaceship spaceship)
    {
        Transform newShip = Instantiate(shipPrefab, centralStar.orbiting).transform;
        newShip.GetComponent<SpaceShipGO>().spaceship = spaceship;
    }

    /// <summary>
    /// Bring up screen allowing player to choose new building. 
    /// </summary>
    public void InitiateBuildingSelection()
    {
        buildingSelector.gameObject.SetActive(true); 
    }

    /// <summary>
    /// Change the colony being selected in the colony summary screen
    /// </summary>
    /// <param name="reference">Index reference to the colony in colonies.</param>
    public void SetSelectedColony(int reference)
    {
        StateManager.currentSM.currentSession.SetSelectedColony(reference); 
    }

    /// <summary>
    /// Change the current time passage mode.
    /// </summary>
    /// <param name="index">New time mode</param>
    public void SetTimeMode(int index)
    {
        StateManager.currentSM.currentSession.SetTimeMode(index);
    }

    /// <summary>
    /// Display a message to the player.
    /// </summary>
    /// <param name="message">Message to be displayed.</param>
    public void DisplayMessage(string message)
    {
        Transform newMessage = Instantiate(messagePrefab, messageParent).transform;

        newMessage.GetComponentInChildren<Text>().text = message;
    }

    /// <summary>
    /// Open/close refineries to control fuel production. 
    /// </summary>
    public void ToggleRefinery()
    {
        StateManager.currentSM.currentSession.currentColony.isRefineryActive = !StateManager.currentSM.currentSession.currentColony.isRefineryActive; 
    }

    /// <summary>
    /// Debug tool to instantly research colony's minerals.
    /// </summary>
    public void Survey()
    {
        StateManager.currentSM.currentSession.currentColony.isSurveyed = true;
    }

    /// <summary>
    /// Quit the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit(); 
    }
}