using UnityEngine;
using UnityEngine.UI;
using Ships;

namespace Economy
{

    /// <summary>
    /// Updates the colony summary screen.
    /// </summary>
    public class ColonyScreen : MonoBehaviour
    {
        /// <summary>
        /// Text references on the summary screen.
        /// </summary>
        [SerializeField]
        private Text topLeftHeader, minerals, mineralStockpiles, production, shipyard, refineries;

        /// <summary>
        /// Ship cost text elements.
        /// </summary>
        [SerializeField]
        private Text cargoCost, colonyCost, tankerCost, surveyCost;

        /// <summary>
        /// The parent GameObject to which all list elements are added to. 
        /// </summary>
        [SerializeField]
        private Transform parent;

        /// <summary>
        /// The prefab object for an entry in the list of colonies.
        /// </summary>
        [SerializeField]
        private GameObject colonyButtonPrefab;

        /// <summary>
        /// Reference to the screen title text.
        /// </summary>
        [SerializeField]
        private Text title;

        /// <summary>
        /// Currently colony being selected.
        /// </summary>
        public Colony colony;

        /// <summary>
        /// Screen to be displayed when no colony is selected. 
        /// </summary>
        public Transform emptyScreen;

        /// <summary>
        /// Default colony summary screen.
        /// </summary>
        public Transform fullScreen;

        /// <summary>
        /// When colony summary screen is shown.
        /// </summary>
        public void OnEnable()
        {
            foreach (Transform child in parent)
                Destroy(child.gameObject);

            // Make a UI button for each colony that the player possesses 
            for (int i = 0; i < StateManager.currentSM.currentSession.colonies.Count; i++)
            {
                Colony colony = StateManager.currentSM.currentSession.colonies[i];
                int j = 0;
                j = i;

                Transform newButton = Instantiate(colonyButtonPrefab, parent).transform;

                newButton.GetComponentInChildren<Text>().text = colony.PlanetaryBody + (colony.isHabitable ? " (H) " : "");

                newButton.GetComponent<Button>().onClick.AddListener(() => UIManager.current.SetSelectedColony(j));
            }

            cargoCost.text = "Min Cost: " + Minerals.Resources(Spaceship.MineralCosts[Spaceship.ShipType.Cargo]) + " Build pts: " + StateManager.currentSM.currentSession.BuildCostPerShip.ToString("N0");
            tankerCost.text = "Min Cost: " + Minerals.Resources(Spaceship.MineralCosts[Spaceship.ShipType.Tanker]) + " Build pts: " + StateManager.currentSM.currentSession.BuildCostPerShip.ToString("N0");
            surveyCost.text = "Min Cost: " + Minerals.Resources(Spaceship.MineralCosts[Spaceship.ShipType.Survey]) + " Build pts: " + StateManager.currentSM.currentSession.BuildCostPerShip.ToString("N0");
            colonyCost.text = "Min Cost: " + Minerals.Resources(Spaceship.MineralCosts[Spaceship.ShipType.Colony]) + " Build pts: " + StateManager.currentSM.currentSession.BuildCostPerShip.ToString("N0");

        }

        /// <summary>
        /// Update per frame.
        /// </summary>
        public void FixedUpdate()
        {
            int sc = StateManager.currentSM.currentSession.SelectedColony;
            if (sc >= StateManager.currentSM.currentSession.colonies.Count)
            {
                fullScreen.gameObject.SetActive(false);
                emptyScreen.gameObject.SetActive(true);
                return;
            }
            fullScreen.gameObject.SetActive(true);
            emptyScreen.gameObject.SetActive(false);
            colony = StateManager.currentSM.currentSession.colonies[sc];

            title.text = "Colony Summary: " + colony.PlanetaryBody;

            topLeftHeader.text = (!colony.isHabitable ? "UNSUITABLE FOR HUMAN HABITATION\n" : "Population : " + colony.Population.ToString("N0") + "\n") +
                "Required Population : " + colony.RequiredPopulation + "\n" +
                "Happiness : " + colony.Happiness.ToString("N0") + "\n" +
                "\n" +
                "Factories (" + colony.Infrastructure[Buildable.BuildingType.Factory] + ")\n" +
                "Mine (" + colony.Infrastructure[Buildable.BuildingType.Mine] + ")\n" +
                "Automated Mine (" + colony.Infrastructure[Buildable.BuildingType.Automated_Mine] + ")\n" +
                "Research Lab (" + colony.Infrastructure[Buildable.BuildingType.Research_Lab] + ")\n" +
                "Fuel Refinery (" + colony.Infrastructure[Buildable.BuildingType.Fuel_Refinery] + ")\n" +
                "Financial Center (" + colony.Infrastructure[Buildable.BuildingType.Financial_Centre] + ")\n" +
                "Shipyard (" + colony.Infrastructure[Buildable.BuildingType.Shipyard] + ")\n" +
                "Leisure (" + colony.Infrastructure[Buildable.BuildingType.Leisure] + ")\n";

            string reserves = "", stockpile = "";
            for (int i = 0; i < Minerals.MINERALS_NAME.Length; i++)
            {
                reserves += (colony.PlanetaryResources.minerals[i]).ToString("N0") + " (" + (colony.PlanetaryResources.accessibility[i]).ToString("F1") + ")\n";
                stockpile += colony.MineralStockpiles[i].ToString("N0") + "\n";
            }

            minerals.text = colony.isSurveyed ? reserves : "Minerals\nnot\nsurveyed.";
            mineralStockpiles.text = stockpile;

            production.text = "# of Factories : " + colony.Infrastructure[Buildable.BuildingType.Factory] + "\n" +
                "Production Pts: " + colony.GetFactoryProduction.ToString("N0") + "\n" +
                "Currently Producing: " + (colony.CurrentFactoryProject == null ? "Nothing" : colony.CurrentFactoryProject.name + "\n(" + (colony.RemainingFactoryTime != -1 ? colony.RemainingFactoryTime.ToString("N0") + " days)" : ">999 days)"));

            shipyard.text = "# of Shipyard Facilities : " + colony.Infrastructure[Buildable.BuildingType.Shipyard] + "\n" +
                "Production Pts: " + colony.GetShipyardProduction.ToString("N0") + "\n" +
                "Currently Producing: " + (Spaceship.ShipTypeNames[(int)colony.CurrentShipyardProject]) + (colony.CurrentShipyardProject == Spaceship.ShipType.Null ? "" : " (" + (colony.RemainingShipyardTime != -1 ? colony.RemainingShipyardTime.ToString("N0") + " days)" : ">999 days)"));

            refineries.text = "# of Refinery Facilities : " + colony.Infrastructure[Buildable.BuildingType.Fuel_Refinery] + (colony.isRefineryActive ? " (ACTIVE)" : " (INACTIVE)") +  "\n" +
                "Fuel Storage: " + colony.FuelStockpile.ToString("N0") + " litres";
        }

        public void BuildShip(int shiptype)
        {
            colony.CurrentShipyardProject = (Spaceship.ShipType)shiptype;
            Debug.Log("Going to construct : " + Spaceship.ShipTypeNames[shiptype]);
        }
    }
}
