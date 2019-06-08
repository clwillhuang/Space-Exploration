namespace Economy {

    /// <summary>
    /// Class for a building that a player can build on a colony.
    /// </summary>
    public class Buildable {

        /// <summary>
        /// The types of buildings.
        /// </summary>
        public enum BuildingType { Fuel_Refinery, Mine, Automated_Mine, Research_Lab, Factory, Shipyard, Leisure, Financial_Centre };
        
        /// <summary>
        /// The displayed name to each building type.
        /// </summary>
        public static readonly string[] BUILDING_NAMES = { "Fuel Refinery", "Mine", "Automated Mine", "Research Lab", "Factory", "Shipyard", "Leisure", "Financial Centre" };  

        /// <summary>
        /// The number of possible buildings.
        /// </summary>
        public static int NumOfBuildings {
            get {
                return BUILDING_NAMES.Length; 
            }
        }
        
        /// <summary>
        /// This building's type.
        /// </summary>
        public readonly BuildingType buildingType; 

        /// <summary>
        /// The population required to run this building at full efficiency. 
        /// </summary>
        public int workerPopulation { get; private set; }

        /// <summary>
        /// The name of this building.
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// The money price to build this. 
        /// </summary>
        public int money { get; private set; }

        /// <summary>
        /// The build points to build this. 
        /// </summary>
        public float buildPoints { get; private set; }

        /// <summary>
        /// The required resources to build this building. 
        /// </summary>
        public float[] requiredResources { get; private set; } 

        /// <summary>
        /// Output the resources required.
        /// </summary>
        /// <returns>A list string bounded by [ ]</returns>
        public string Resources()
        {
            string r = "[";

            for (int i = 0; i < Minerals.MINERALS_NAME.Length; i++)
            {
                r += requiredResources[i];
                r += (i == Minerals.MINERALS_NAME.Length - 1 ? "]" : ", ");
            }

            return r; 
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="_buildingType">The building type this is.</param>
        /// <param name="_workerPopulation">The required population to operate this building.</param>
        /// <param name="_name">Building name</param>
        /// <param name="_price">Money required to build..</param>
        /// <param name="_buildPoints">Build points required to build.</param>
        /// <param name="_requiredResources">Required resources to build.</param>
        public Buildable(BuildingType _buildingType, int _workerPopulation, string _name, int _price, float _buildPoints, float[] _requiredResources)
        {
            money = _price;
            buildingType = _buildingType; 
            workerPopulation = _workerPopulation;
            name = _name;
            buildPoints = _buildPoints;
            requiredResources = _requiredResources; 
        }
    }
}
