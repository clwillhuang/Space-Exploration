using UnityEngine;
using UnityEngine.UI; 

/// <summary>
/// Updates the building selection button when it is shown.
/// </summary>
public class BuildingOption : MonoBehaviour {

    public int option; 

    private void Start()
    {
        option = transform.GetSiblingIndex();

        Economy.Buildable b = StateManager.buildableBuildings[option];

        Text[] t = GetComponentsInChildren<Text>(); 

        t[0].text = b.name + " (" + b.money.ToString("C0") + ")";
        t[1].text = "Required resources " + b.Resources() + " / " + b.buildPoints.ToString("N0") + " Build Points.";

        GetComponent<Button>().onClick.AddListener(() => StateManager.currentSM.SelectBuilding(option));

    }

}
