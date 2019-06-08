using System.Collections.Generic;
using UnityEngine; 

namespace Space
{
    /// <summary>
    /// Class for a star system.
    /// </summary>
    [System.Serializable]
    public class StarSystem
    {
        /// <summary>
        /// Possible system name prefixes. 
        /// </summary>
        private List<string> systemNames = new List<string>()
        {
            "Wolf",
            "Luyten",
            "Gliese",
            "HIP",
            "HD",
            "WX",
            "Cygni",
            "Alpha",
            "HR",
            "EQ",
            "Struve",
            "Lalande",
            "GJ",
            "SCR"
        };

        /// <summary>
        /// Roman numerals from 1 to 10. 
        /// </summary>
        private readonly string[] RomanNumerals = new string[10] { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };

        /// <summary>
        /// The name of this system.
        /// </summary>
        public readonly string SystemName; 

        /// <summary>
        /// The index of star system within Player.galaxy
        /// </summary>
        public readonly int index; 

        /// <summary>
        /// The central star in this system. 
        /// </summary>
        public readonly SystemBody star;

        /// <summary>
        /// The Jump Points present in this star system. 
        /// </summary>
        public List<JumpPoint> JumpPoints { get; set; }

        /// <summary>
        /// The number of planets in this star system.
        /// </summary>
        private readonly int numPlanets; 

        /// <summary>
        /// Constructor for this star system.
        /// </summary>
        /// <param name="newIndex">Index of this star system in Player.galaxy </param>
        /// <param name="previous">Existing star system that this system will connect to.</param>
        /// <param name="jp">Existing jump point linking new system to existing system.</param>
        public StarSystem(int newIndex, int previous = -1, int jp = -1)
        {
            // Generate System

            index = newIndex;

            // Generate unique system name 
            SystemName = systemNames[Random.Range(0, systemNames.Count - 1)] + " " + Random.Range(100, 999);
            while (StateManager.currentSM.currentSession.systemNames.Contains(SystemName))
                SystemName = systemNames[Random.Range(0, systemNames.Count - 1)] + " " + Random.Range(100, 999);
            StateManager.currentSM.currentSession.systemNames.Add(SystemName);

            // Generate star
            star = new Star(SystemName, 0);
            numPlanets = newIndex == 0 ? Random.Range(6, 8) : Random.Range(1, 8);

            float planetOrbitDistance = 20f;
            float moonOrbitDistance = 0f;

            int guaranteedHabitablePlanet = Random.Range(0, numPlanets - 1);
            Debug.Log("Guaranteed habitable planet in home system is # " + (guaranteedHabitablePlanet + 1)); 

            // Generate planets
            for (int i = 0; i < numPlanets; i++)
            {
                // Randomize orbit distance from the star.
                planetOrbitDistance += Random.Range(Constants.MAX_COORDINATE_X / numPlanets - 10f, 
                    (Constants.MAX_COORDINATE_X / numPlanets + 10f)); 

                OrbitingBody newPlanet = new OrbitingBody(index, SystemName + " " + RomanNumerals[i], 
                    Random.Range(0f, 100f), 
                    planetOrbitDistance, 
                    SystemBody.SystemBodyType.Planet,
                    guaranteedHabitablePlanet == i && newIndex == 0);

                // Add this planet to orbit around star.
                star.AddNewBody(newPlanet);

                int numMoons = 0;

                moonOrbitDistance = 0f;
            }

            int guaranteedConnection = Random.Range(1, Constants.JUMP_POINTS_PER_SYSTEM - 1); 

            // Generate jump points.
            JumpPoints = new List<JumpPoint>();
            for (int i = 0; i < Constants.JUMP_POINTS_PER_SYSTEM; i++)
            {
                // First jump point will connect to existing system, if necessary
                if (i == 0 && previous != -1) JumpPoints.Add(new JumpPoint(newIndex, i, previous, jp));

                // Last jump point is guaranteed to connect to a new system.
                else if (i == guaranteedConnection) JumpPoints.Add(new JumpPoint(newIndex, i, true));
                
                // Regular jump point
                else JumpPoints.Add(new JumpPoint(newIndex, i));
            }

        }


    }

}