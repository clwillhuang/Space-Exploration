using UnityEngine;
using Economy; 

namespace Space
{
    /// <summary>
    /// Class for an object orbiting another object.
    /// </summary>
    [System.Serializable]
    public class OrbitingBody : SystemBody
    {
        /// <summary>
        /// The system that this planet is in.
        /// </summary>
        public int systemLocationIndex;

        /// <summary>
        /// The colony present on this orbiting object.
        /// </summary>
        [SerializeField]
        public Colony colony { get; private set; }

        /// <summary>
        /// The orbital distance, in km, from parent star or planet
        /// </summary>
        [SerializeField]
        private float orbitDistance { get; set; }

        /// <summary>
        /// Randomized speed at which body orbits, determining orbital period.
        /// </summary>
        [SerializeField]
        private float orbitalFactor = 0f;

        /// <summary>
        /// The minerals present on this object.
        /// </summary>
        [SerializeField]
        public Minerals BodyMinerals;

        /// <summary>
        /// Is this planet habitable? Determined during system generation. 
        /// </summary>
        public bool isHabitable { get; private set; }

        /// <summary>
        /// The x-coordinate of this object in the system.
        /// </summary>
        /// <returns>X coordinate</returns>
        override public float CoordX()
        {
            return orbitDistance * Mathf.Sin(2 * Mathf.PI * OrbitalPercentage);
        }

        /// <summary>
        /// The y-coordinate of this object in the system.
        /// </summary>
        /// <returns>Y coordinate</returns>
        override public float CoordY()
        {
            return orbitDistance * Mathf.Cos(2 * Mathf.PI * OrbitalPercentage);
        }

        /// <summary>
        /// The percentage of the current orbit that this object has completed.
        /// </summary>
        public float OrbitalPercentage { get; private set; }

        /// <summary>
        /// Movement update.
        /// </summary>
        override public void Update()
        {
            foreach (OrbitingBody satellite in OrbitingPlanets)
            {
                satellite.Update();
            }

            OrbitalPercentage = OrbitalPercentage + Time.deltaTime / (orbitalFactor * orbitDistance) * StateManager.currentSM.currentSession.TimeMode / 2f;
            if (OrbitalPercentage > 1f)
            {
                OrbitalPercentage = OrbitalPercentage - 1f;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_name">The name of this object.</param>
        /// <param name="_mass">The mass, in 10^24 kilograms</param>
        /// <param name="_orbitDistance">The orbital distance, in km, from parent star or planet</param>
        /// <param name="_systemBodyType">The type of object this is.</param>
        public OrbitingBody(int _systemLocationIndex, string _name, float _mass, float _orbitDistance, SystemBodyType _systemBodyType, bool _isHabitable = false) : base(_name, _mass, _systemBodyType)
        {
            colony = null;

            systemLocationIndex = _systemLocationIndex; 

            orbitalFactor = Random.Range(1f, 5f);

            orbitDistance = _orbitDistance;

            OrbitalPercentage = Random.Range(0f, 1f); 

            BodyMinerals = new Minerals();

            BodyMinerals.RandomizeResources((float)mass, _systemLocationIndex == 0 && _isHabitable);

            isHabitable = _isHabitable || (Random.Range(0f, 1f) <= Constants.HABITABLE_PLANET_GENERATION_CHANCE);

            if (isHabitable)
                sprite = UIManager.current.planetsHabitable[Random.Range(0, UIManager.current.planetsHabitable.Count - 1)];
            else 
                sprite = UIManager.current.planetsUninhabitable[Random.Range(0, UIManager.current.planetsUninhabitable.Count - 1)];
        }

        /// <summary>
        /// Establish a colony on this celestial object.
        /// </summary>
        public void EstablishColony()
        {
            colony = new Colony(this, LocationName, BodyMinerals, isHabitable, isSurveyed, systemLocationIndex);
            StateManager.currentSM.currentSession.AddColony(colony);
            UIManager.current.DisplayMessage("A colony has been established on " + LocationName);
        }
        

        /// <summary>
        /// Establish the player's homeworld (first colony) on this planet.
        /// </summary>
        /// <returns></returns>
        public Colony EstablishHomeWorld()
        {
            UIManager.current.DisplayMessage("Home world established on " + LocationName);
            colony = new Colony(this, LocationName, BodyMinerals, true, true, systemLocationIndex, true);
            return colony;
        }

        /// <summary>
        /// Conduct a geological survey on this system body. Displays appropriate message.
        /// </summary>
        public override void Survey() {

            isSurveyed = true;
            if (colony != null) colony.isSurveyed = true; 

            bool hasMinerals = false;
            for (int i = 0; i < Minerals.MINERALS_NAME.Length && !hasMinerals; i++)
                if (BodyMinerals.minerals[i] != 0) hasMinerals = true; 

            if (!hasMinerals) {
                UIManager.current.DisplayMessage(LocationName + " surveyed! No resources were found."); 
                return;
            }

            string r = "";

            for (int i = 0; i < Minerals.MINERALS_NAME.Length; i++)
                if (BodyMinerals.accessibility[i] > 0f || BodyMinerals.minerals[i] > 0f)
                    r += (r == "" ? "" : ", ") + Minerals.MINERALS_NAME[i];
            
            UIManager.current.DisplayMessage(LocationName + " surveyed! Resources found: " + r); 
        }
    }
}
