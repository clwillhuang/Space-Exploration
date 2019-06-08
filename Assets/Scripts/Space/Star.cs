using UnityEngine;

namespace Space
{
    /// <summary>
    /// A star.
    /// </summary>
    public class Star : SystemBody
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_name">The name of this object.</param>
        /// <param name="_mass">The mass, in 10^24 kilograms</param>
        public Star(string _name, float _mass) : base(_name, _mass, SystemBody.SystemBodyType.Star)
        {
            sprite = UIManager.current.stars[Random.Range(0, UIManager.current.stars.Count - 1)];
        }

        public override void Survey()
        {
            UIManager.current.DisplayMessage("Cannot survey stars. No minerals are present on stars.");
        }
    }
}
