using UnityEngine;
using UnityEngine.UI;

namespace Ships {

    /// <summary>
    /// Show and update a spaceship icon on screen.
    /// </summary>
    public class SpaceShipGO : MonoBehaviour {

        /// <summary>
        /// The different sprites for each ship type.
        /// </summary>
        [SerializeField]
        private Sprite ColonyShipSprite, CargoShipSprite, TankerShipSprite, SurveyShipSprite;

        /// <summary>
        /// The spaceship that this gameobject is representing.
        /// </summary>
        public Spaceship _spaceship;

        /// <summary>
        /// The spaceship that this gameobject is representing.
        /// </summary>
        public Spaceship spaceship
        {
            get
            {
                return _spaceship; 
            }
            set
            {
                _spaceship = value;

                if (value != null)
                    gameObject.SetActive(true);

                if (spaceship.shiptype == Spaceship.ShipType.Cargo)
                    GetComponentInChildren<SpriteRenderer>().sprite = CargoShipSprite; 
                else if (spaceship.shiptype == Spaceship.ShipType.Colony)
                    GetComponentInChildren<SpriteRenderer>().sprite = ColonyShipSprite;
                else if (spaceship.shiptype == Spaceship.ShipType.Tanker)
                    GetComponentInChildren<SpriteRenderer>().sprite = TankerShipSprite;
                else if(spaceship.shiptype == Spaceship.ShipType.Survey)
                    GetComponentInChildren<SpriteRenderer>().sprite = SurveyShipSprite;
            }
        }

        /// <summary>
        /// Update every frame.
        /// </summary>
        public void Update()
        {
            if (spaceship != null)
            {
                if (spaceship.orbitingObject == null)
                    transform.localPosition = new Vector3(spaceship.localPositionInSystem.x, spaceship.localPositionInSystem.y, 0f);
                else
                    transform.localPosition = new Vector3(spaceship.orbitingObject.CoordX(), spaceship.orbitingObject.CoordY(), 0f);

                GetComponentInChildren<Text>().text = spaceship.ShipName + "\nF" + (spaceship.FuelOnBoard / spaceship.FuelCapacity * 100).ToString("N0") + "%/C" 
                    + (spaceship.Condition).ToString("N0") + "%";
            }
            else
            {
                gameObject.SetActive(false);
            }
            if (spaceship.systemLocationIndex != UIManager.current.CurrentlyDisplayedSystem)
            {
                Destroy(gameObject);
                return; 
            }
        }

    }

}