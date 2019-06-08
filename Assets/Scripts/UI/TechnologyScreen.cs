using Economy;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Control technology UI screen.
/// </summary>
public class TechnologyScreen : MonoBehaviour {

    /// <summary>
    /// Index of the currently selected tech in the UI list.
    /// </summary>
    private int selectedIndex;

    /// <summary>
    /// Currently selected tech in the UI list
    /// </summary>
    private Technology selectedTech;

    /// <summary>
    /// Prefab object for tech entry in list.
    /// </summary>
    [SerializeField]
    private GameObject technologyPrefab; 
    
    /// <summary>
    /// Parent gameobject of the list.
    /// </summary>
    [SerializeField]
    private Transform technologyParent;

    /// <summary>
    /// Displayed text elements.
    /// </summary>
    [SerializeField]
    private Text title, description, rate, current;  

    /// <summary>
    /// The unlock button used to change what tech is being researched.
    /// </summary>
    [SerializeField]
    private Button unlockButton;

    /// <summary>
    /// Index of the tech currently being researched by the player.
    /// </summary>
    private int currentResearch;

    /// <summary>
    /// Construct the technology list, technology screen appears on screen. 
    /// </summary>
    public void OnEnable()
    {
        foreach (Transform child in technologyParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < StateManager.availableTechnologies.Count; i++)
        {
            Technology technology = StateManager.availableTechnologies[i];

            string name = technology.technologyName;
            Transform newButton = Instantiate(technologyPrefab, technologyParent).transform;

            newButton.GetComponentInChildren<Text>().text = name;

            int f = i;
            newButton.GetComponent<Button>().onClick.AddListener(() => SelectTechnology(f));
        }

        SelectTechnology(0);
    }

    private void FixedUpdate()
    {
        rate.text = "# of Research Labs: " + StateManager.currentSM.currentSession.ResearchLabCount + "     " 
            + "Daily Research Rate: " + (StateManager.currentSM.currentSession.ResearchSpeedPerResearchLab * StateManager.currentSM.currentSession.ResearchLabCount).ToString("N0") + " / day";
        currentResearch = StateManager.currentSM.currentSession.CurrentResearch;
        if (currentResearch == -1)
            current.text = "Not researching anything right now.";
        else
            current.text = StateManager.availableTechnologies[currentResearch].technologyName + "\n(" + (StateManager.currentSM.currentSession.technologyProgress[currentResearch]) + " research pts left.";

        title.text = selectedTech.technologyName;

        string desc = selectedTech.description + "\n\n";
            desc += "Prerequisite tech: \n\t\t" + (selectedTech.prerequisiteTech == "" ? "None" : selectedTech.prerequisiteTech) + "\n";
        if (StateManager.currentSM.currentSession.researchedTechnologies.Contains(selectedTech.technologyName))
            desc += "(RESEARCHED)\n";
        desc += "Cost: \n\t\t" + selectedTech.cost.ToString("N0");
        description.text = desc;

        unlockButton.interactable = StateManager.currentSM.currentSession.researchedTechnologies.Contains(StateManager.availableTechnologies[selectedIndex].prerequisiteTech) && 
            !StateManager.currentSM.currentSession.researchedTechnologies.Contains(StateManager.availableTechnologies[selectedIndex].technologyName);
    }

    /// <summary>
    /// Select the technology to be displayed.
    /// </summary>
    /// <param name="index">The index of the technology within the available technology list.</param>
    public void SelectTechnology(int index)
    {
        Debug.Log("Selected " + index);

        selectedIndex = index;
        selectedTech = StateManager.availableTechnologies[index];
    }

    /// <summary>
    /// Begin research of the selected technology.
    /// </summary>
    public void SelectResearch()
    {
        StateManager.currentSM.currentSession.CurrentResearch = selectedIndex;
    }
}
