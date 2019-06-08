using UnityEngine;

[System.Serializable]
public class Minerals
{
    /// <summary>
    /// The names of the minerals in this game. 
    /// </summary>
    public static readonly string[] MINERALS_NAME = 
        { "Duranium",
        "Neutronium",
        "Tritanium",
        "Corundium",
        "Corbomite",
        "Sorium",
        "Gallicite",
        "Uridium" };
    
    public static int NumMinerals
    {
        get { return MINERALS_NAME.Length; }
    }

    /// <summary>
    /// Enum for minerals.
    /// </summary>
    public enum MINERALS_ENUM { Duranium = 0, Neutronium, Tritanium, Corundium, Corbomite, Sorium, Gallicite, Uridium }; 

    /// <summary>
    /// The impact of planet size on mineral abundance.
    /// </summary>
    private const float SizeFactor = 100000f;

    //private const float AbundanceFactor = 100000f; 

    /// <summary>
    /// The quantity of minerals present. 
    /// </summary>
    [SerializeField]
    public float[] minerals = new float[MINERALS_NAME.Length];

    /// <summary>
    /// The accessibility of each mineral, 0f - 1f, determining how fast it is mined.
    /// </summary>
    [SerializeField]
    public float[] accessibility = new float[MINERALS_NAME.Length];

    /// <summary>
    /// Default constructor.
    /// </summary>
    public Minerals()
    {
        for (int i = 0; i < MINERALS_NAME.Length; i++)
        {
            minerals[i] = 0f;
            accessibility[i] = 0f; 
        }
    }

    /// <summary>
    /// Initialize mineral generation. 
    /// </summary>
    /// <param name="size">The mass of this planet.</param>
    public void RandomizeResources(float size, bool homeworld = false)
    {
        if (!homeworld)
        {
            float richness = Mathf.Clamp(Random.Range(0f, 1f), 0f, size / SizeFactor);

            // For each mineral, randomly generate quantity and accessibility of resources.
            for (int i = 0; i < MINERALS_NAME.Length; i++)
            {
                if (Random.Range(0f, 1f) < richness)
                {
                    minerals[i] = 0f;
                    accessibility[i] = 0f;
                }
                else
                {
                    minerals[i] = 0f;
                    while (Random.Range(0f, 1f) > 0.5f)
                        minerals[i] += Random.Range(0, 100000);
                    if (minerals[i] > 0f) accessibility[i] = Random.Range(1, 10) / 10f;
                    else accessibility[i] = 0f;
                }
            }
        }
        else
        {
            for (int i = 0; i < MINERALS_NAME.Length; i++)
            {
                minerals[i] = Random.Range(100000, 200000) / 10000 * 10000f;
                while (Random.Range(0f, 1f) > 0.2f)
                    minerals[i] += Random.Range(10000, 20000);
                accessibility[i] = Random.Range(4, 10) / 10f;

            }
        }
    }

    /// <summary>
    /// Check if there are sufficient resources for something (i.e. building a building)
    /// </summary>
    /// <param name="current">The resources possessed by the player.</param>
    /// <param name="required">The required resources.</param>
    /// <returns></returns>
    public static bool IsSufficient(float[] current, float[] required)
    {
        if (current.Length != required.Length)
        {
            Debug.LogError("Minerals.cs: Invalid lengths: " + current.Length + " " + required.Length); 
        }

        for (int i = 0; i < current.Length; i++)
        {
            // Break if there is one mineral that is insufficient.
            if (current[i] < required[i])
            {
                return false; 
            }
        }
        // Sufficient minerals present.
        return true; 
    }
    
    /// <summary>
    /// Calculate the quantity of minerals mined per day.
    /// </summary>
    /// <param name="miningProductivity">The maximal number of tons mined per resource per day, as determined by mines on a colony.</param>
    /// <returns>The tons of each mineral mined.</returns>
    public float[] MineTick(float miningProductivity) {
        
        float[] productivity = new float[MINERALS_NAME.Length]; 
        
        for (int i = 0; i < minerals.Length; i++)
        {
            // Calculate the maximum tonnage mined 
            float tons = accessibility[i] * miningProductivity; 
            productivity[i] = Mathf.Min(tons, minerals[i]); 

            // Subtract mined resources from mineral reserves.
            minerals[i] = Mathf.Max(minerals[i] - productivity[i], 0f); 
        }
        
        // Return the quantity of each mineral mined.
        return productivity;
    }

    /// <summary>
    /// Output the resources required.
    /// </summary>
    /// <returns>A list string bounded by [ ]</returns>
    public static string Resources(float[] resources)
    {
        string r = "[";

        for (int i = 0; i < Mathf.Min(MINERALS_NAME.Length, resources.Length); i++)
        {
            r += resources[i];
            r += (i == Mathf.Min(MINERALS_NAME.Length, resources.Length) - 1 ? "]" : ", ");
        }

        return r;
    }
}
