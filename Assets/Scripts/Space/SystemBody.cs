using System.Collections.Generic;
using UnityEngine;

namespace Space
{
    /// <summary>
    /// Base class for a system body (planet or star).
    /// </summary>
    [System.Serializable]
    public abstract class SystemBody : Point
    {
        /// <summary>
        /// The type of celestial body this object is.
        /// </summary>
        public enum SystemBodyType { Star, Planet, Moon };

        /// <summary>
        /// The sprite for this body.
        /// </summary>
        public Sprite sprite { get; protected set; }

        /// <summary>
        /// The type of celestial object this is. 
        /// </summary>
        public readonly SystemBodyType bodyType; 

        /// <summary>
        /// Mass, as measured in 10^24 kilograms 
        /// </summary>
        public readonly float mass; 

        public List<OrbitingBody> OrbitingPlanets { get; protected set; }

        /// <summary>
        /// The x-coordinate of this object in the system.
        /// </summary>
        /// <returns>X coordinate</returns>
        public override float CoordX()
        {
            return 0f;
        }

        /// <summary>
        /// The y-coordinate of this object in the system.
        /// </summary>
        /// <returns>Y coordinate</returns>
        public override float CoordY()
        {
            return 0f;
        }

        /// <summary>
        /// Update location in space.
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
        /// Add a new system body orbiting around this object.
        /// </summary>
        /// <param name="newSystemBody">New orbiting system body.</param>
        public void AddNewBody(OrbitingBody newSystemBody)
        {
            OrbitingPlanets.Add(newSystemBody);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_bodyName">The name of this object.</param>
        /// <param name="_mass">The mass, in 10^24 kilograms</param>
        /// <param name="_systemBodyType">The type of object this is.</param>
        public SystemBody(string _bodyName, float _mass, SystemBodyType _systemBodyType) : base(_bodyName, (_systemBodyType != SystemBodyType.Star ? _mass : 0f) * Constants.PLANET_SURVEY_POINTS_PER_1023)
        {
            LocationName = _bodyName;
            mass = _mass;
            bodyType = _systemBodyType;
            OrbitingPlanets = new List<OrbitingBody>();
            sprite = null;
        }

    }

}
