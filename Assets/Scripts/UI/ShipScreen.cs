using UnityEngine;
using UnityEngine.UI;
using Ships;
using Space;
using System.Linq;
using Economy; 

/// <summary>
/// Update the ship interactions screen.
/// </summary>
public class ShipScreen : MonoBehaviour {

    [SerializeField]
    private GameObject shipButtonPrefab, bodyButtonPrefab, orderTypePrefab;

    [SerializeField]
    private Transform shipButtonParent, bodyButtonParent, orderTypeParent;

    [SerializeField]
    private Dropdown mineralDropdown, buildingDropdown;

    [SerializeField]
    private GameObject loadMineral, loadBuilding;

    /// <summary>
    /// The ship being currently selected.
    /// </summary>
    private int selectedShipIndex;

    /// <summary>
    /// The current system body being selected for an order.
    /// </summary>
    private int selectedBody;

    [SerializeField]
    private Text orders, info;

    /// <summary>
    /// The currently selected spaceship.
    /// </summary>
    private Spaceship spaceship;

    public void Start()
    {
        mineralDropdown.ClearOptions();
        mineralDropdown.AddOptions(Minerals.MINERALS_NAME.ToList());
        buildingDropdown.ClearOptions();
        buildingDropdown.AddOptions(Buildable.BUILDING_NAMES.ToList());
    }

    /// <summary>
    /// When screen is shown.
    /// </summary>
    public void OnEnable()
    {
        UpdateShipList(); 
        UpdateSystemBodyList();
        UpdateOrderList();

    }

    /// <summary>
    /// Update the list of ships displayed to player.
    /// </summary>
    public void UpdateShipList()
    {
        foreach (Transform child in shipButtonParent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < StateManager.currentSM.currentSession.spaceships.Count; i++)
        {
            Spaceship ship = StateManager.currentSM.currentSession.spaceships[i];
            Transform newButton = Instantiate(shipButtonPrefab, shipButtonParent).transform;
            newButton.GetComponentInChildren<Text>().text = ship.ShipName;
            int f = i;
            newButton.GetComponent<Button>().onClick.AddListener(() => SelectShip(f));
        }

        if (StateManager.currentSM.currentSession.spaceships.Count == 0)
        {
            spaceship = null; selectedShipIndex = 0;
        }
        else
        {
            selectedShipIndex = 0;
            spaceship = StateManager.currentSM.currentSession.spaceships[selectedShipIndex];
        }
    }

    /// <summary>
    /// Update the displayed list of system bodies that the currently selected ship can visit.
    /// </summary>
    public void UpdateSystemBodyList()
    {
        foreach (Transform child in bodyButtonParent)
        {
            Destroy(child.gameObject);
        }

        if (spaceship == null) return;

        // add planets
        for (int i = 0; i < StateManager.currentSM.currentSession.galaxy[spaceship.lastSystemLocationIndex].star.OrbitingPlanets.Count; i++)
        {
            OrbitingBody planet = StateManager.currentSM.currentSession.galaxy[spaceship.lastSystemLocationIndex].star.OrbitingPlanets[i];
            Transform newButton = Instantiate(bodyButtonPrefab, bodyButtonParent).transform;
            newButton.GetComponentInChildren<Text>().text = planet.LocationName +
                (planet.colony != null ? " (C)" : "") +
                (planet.isHabitable ? " (H) " : ""); 
            int f = i;
            newButton.GetComponent<Button>().onClick.AddListener(() => SelectBody(f, newButton.GetSiblingIndex()));

            if (i == 0) newButton.GetComponent<Button>().interactable = false;
        }
        // leave first selected by default
        selectedBody = 0;
        bodyButtonParent.GetChild(selectedBody).GetComponent<Button>().interactable = true;

        // add jump points
        int numPlanets = StateManager.currentSM.currentSession.galaxy[spaceship.lastSystemLocationIndex].star.OrbitingPlanets.Count; 
        for (int i = 0; i < StateManager.currentSM.currentSession.galaxy[spaceship.lastSystemLocationIndex].JumpPoints.Count; i++)
        {
            Point point = StateManager.currentSM.currentSession.galaxy[spaceship.lastSystemLocationIndex].JumpPoints[i];
            if (!point.isSurveyed && spaceship.shiptype != Spaceship.ShipType.Survey) continue;
            else if (point.isSurveyed && !StateManager.currentSM.currentSession.galaxy[spaceship.lastSystemLocationIndex].JumpPoints[i].IsValidConnection()) continue; 
            Transform newButton = Instantiate(bodyButtonPrefab, bodyButtonParent).transform;
            newButton.GetComponentInChildren<Text>().text = point.LocationName;
            int f = i + numPlanets;
            newButton.GetComponent<Button>().onClick.AddListener(() => SelectBody(f, newButton.GetSiblingIndex()));
        }
    }

    /// <summary>
    /// Delete the last order.
    /// </summary>
    public void DeleteLast()
    {
        if (spaceship != null)
            spaceship.DeleteLast(); 
    }

    /// <summary>
    /// Add a new order to the currently selected ship.
    /// </summary>
    /// <param name="type">A number representing what kind of order is being added.</param>
    public void AddOrder(int type)
    {
        int numBody = StateManager.currentSM.currentSession.galaxy[spaceship.lastSystemLocationIndex].star.OrbitingPlanets.Count; 
        // is this a planet?
        if (selectedBody >= 0 && selectedBody < numBody)
        {
            StateManager.currentSM.currentSession.spaceships[selectedShipIndex].AddOrder(
                StateManager.currentSM.currentSession.galaxy[spaceship.lastSystemLocationIndex].star.OrbitingPlanets[selectedBody],
                type, spaceship.lastSystemLocationIndex);
        }
        // a jump point otherwise
        else if (selectedBody >= numBody && 
            selectedBody < numBody + StateManager.currentSM.currentSession.galaxy[spaceship.lastSystemLocationIndex].JumpPoints.Count)
        {
            int jpindex = selectedBody -= numBody;
            Debug.Log("Selected " + StateManager.currentSM.currentSession.galaxy[spaceship.lastSystemLocationIndex].JumpPoints[jpindex].LocationName);
            StateManager.currentSM.currentSession.spaceships[selectedShipIndex].AddOrder(
                StateManager.currentSM.currentSession.galaxy[spaceship.lastSystemLocationIndex].JumpPoints[jpindex],
                type, spaceship.lastSystemLocationIndex);
        }
        UpdateSystemBodyList(); 
    }

    /// <summary>
    /// Update displayed ship info every frame.
    /// </summary>
    private void Update()
    {
        if (spaceship != null)
        {
            orders.text = spaceship.GetAllOrders();
            info.text = spaceship.GetInfo(); 
        }
        else
        {
            orders.text = "Ship information will be shown here.";
            info.text = "Orders for a ship will be shown here.";
        }
    }

    /// <summary>
    /// Change the ship selected for display.
    /// </summary>
    /// <param name="index">The new ship to be displayed with information.</param>
    public void SelectShip(int index)
    {
        selectedShipIndex = index;
        spaceship = StateManager.currentSM.currentSession.spaceships[selectedShipIndex];
        UpdateSystemBodyList();
        UpdateOrderList(); 
    }

    /// <summary>
    /// Update the displayed list of possible orders that the currently selected ship can fulfill.
    /// </summary>
    public void UpdateOrderList()
    {
        foreach (Transform oldOrder in orderTypeParent)
        {
            Destroy(oldOrder.gameObject);
        }

        if (spaceship == null) return; 

        // If this ship is not a cargo ship
        if (spaceship.shiptype != Spaceship.ShipType.Cargo)
        {
            for (int i = 0; i < spaceship.OrderTypes.Length; i++)
            {
                Transform newOrderType = Instantiate(orderTypePrefab, orderTypeParent).transform;
                newOrderType.GetComponentInChildren<Text>().text = spaceship.OrderTypes[i];
                int f = i;
                newOrderType.GetComponent<Button>().onClick.AddListener(() => AddOrder(f));
            }

            mineralDropdown.gameObject.SetActive(false);
            buildingDropdown.gameObject.SetActive(false);
            loadMineral.SetActive(false);
            loadBuilding.SetActive(false);

        }
        // Manually set all the elements for a cargo ship, due to its loading/unloading mechanics
        else if (spaceship.shiptype == Spaceship.ShipType.Cargo)
        {
            Transform goToType = Instantiate(orderTypePrefab, orderTypeParent).transform;
            goToType.GetComponentInChildren<Text>().text = "Goto";
            goToType.GetComponent<Button>().onClick.AddListener(() => AddOrder(0));

            Transform refuelType = Instantiate(orderTypePrefab, orderTypeParent).transform;
            refuelType.GetComponentInChildren<Text>().text = "Refuel";
            refuelType.GetComponent<Button>().onClick.AddListener(() => AddOrder(1));

            Transform unloadBuildings = Instantiate(orderTypePrefab, orderTypeParent).transform;
            unloadBuildings.GetComponentInChildren<Text>().text = "Unload Building";
            unloadBuildings.GetComponent<Button>().onClick.AddListener(() => AddOrder(2 + Buildable.NumOfBuildings));

            Transform unloadMinerals = Instantiate(orderTypePrefab, orderTypeParent).transform;
            unloadMinerals.GetComponentInChildren<Text>().text = "Unload Minerals";
            unloadMinerals.GetComponent<Button>().onClick.AddListener(() => AddOrder(2 + Buildable.NumOfBuildings + 1 + Minerals.NumMinerals));

            mineralDropdown.gameObject.SetActive(true);
            buildingDropdown.gameObject.SetActive(true);
            loadMineral.SetActive(true);
            loadBuilding.SetActive(true);
        }
    }

    /// <summary>
    /// Select a new system body in the list.
    /// </summary>
    /// <param name="index">The index reference to the system body.</param>
    /// <param name="childIndex">The reference to the gameobject index of the system body in the list of displayed bodies.</param>
    public void SelectBody(int index, int childIndex)
    {
        foreach (Transform body in bodyButtonParent)
        {
            body.GetComponent<Button>().interactable = true;
        }
        bodyButtonParent.GetChild(childIndex).GetComponent<Button>().interactable = false;
        selectedBody = index;
    }

    /// <summary>
    /// Add an order to load a mineral.
    /// </summary>
    public void LoadMineral()
    {
        if (spaceship.shiptype == Spaceship.ShipType.Cargo)
        {
            AddOrder(2 + Buildable.NumOfBuildings + 1 + mineralDropdown.value);
        }
        else
            Debug.LogError("ShipScreen.cs: Mineral loading should be invisible for not cargo.");

    }

    /// <summary>
    /// Add an order to load a building.
    /// </summary>
    public void LoadBuilding()
    {
        if (spaceship.shiptype == Spaceship.ShipType.Cargo)
        {
            AddOrder(2 + buildingDropdown.value);
        }
        else
            Debug.LogError("ShipScreen.cs: Mineral loading should be invisible for not cargo.");
    }

    public void AbandonShip()
    {
        if (spaceship != null)
            spaceship.flaggedForDestruction = true;
    }
}
