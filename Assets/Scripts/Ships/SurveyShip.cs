using UnityEngine;

namespace Ships
{
    /// <summary>
    /// A survey ship.
    /// </summary>
    [System.Serializable]
    public class SurveyShip : Spaceship
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_systemLocationIndex">Index of system this ship was built in.</param>
        /// <param name="_initialOrbitLocation">The initial location of this ship.</param>
        public SurveyShip(int _systemLocationIndex, Point _initialOrbitLocation) :
            base(ShipType.Survey, "Surveyor (C-" + StateManager.currentSM.currentSession.NumberOfShipsEverBuilt + ")", _systemLocationIndex, _initialOrbitLocation)
        {

        }

        /// <summary>
        /// The names for each possible order type for this ship. Varies per ship type.
        /// </summary>
        public override string[] OrderTypes
        {
            get
            {
                return new string[] { "Goto", "Refuel", "Survey" };
            }
        }

        /// <summary>
        /// Get the string representation of an order.
        /// </summary>
        /// <param name="index">The index of the order to be retrieved.</param>
        /// <returns>String representation of the order, complete with type and location.</returns>
        public override string GetOrder(int index)
        {
            int orderType = orders[index].orderType;
            string place = orders[index].point.LocationName;

            if (orderType == 0)
                return "Go to " + place;
            else if (orderType == 1)
                return "Refuel at " + place;
            else if (orderType == 2)
                return "Survey " + place;
            else
                return "Invalid order at " + place; 
        }

        /// <summary>
        /// Process the orders. Once per in-game frame. 
        /// </summary>
        public override void OrderUpdate()
        {
            UpdateConditionAndVerifyOrders();

            if (orders.Count == 0) return;
            Order curr = orders[0];

            // Survey 
            if (curr.orderType == 2)
            {
                if (curr.point.isSurveyed)
                {
                    curr.point.surveyPoints = 0f;
                    Cycle();
                    return; 
                }
                curr.point.surveyPoints -= StateManager.currentSM.currentSession.SurveySpeed * Time.deltaTime * StateManager.currentSM.currentSession.TimeMode; 
                if (curr.point.surveyPoints < 0f)
                {
                    curr.point.surveyPoints = 0f;
                    curr.point.Survey();
                    Cycle();
                    return;
                }

                //Debug.Log("Surveying with " + curr.point.surveyPoints + " left");

            }
            else base.OrderUpdate(); 

        }

        /// <summary>
        /// Get a string description of this spacecraft.
        /// </summary>
        /// <returns>A 4-5 line description including fuel, condition, ship type, etc.</returns>
        public override string GetInfo()
        {
            string r = "";
            if (orders.Count > 0 && orders[0].orderType == 2)
                r = "\nCurrently surveying " + orders[0].point.LocationName + ", pts remaining: " + orders[0].point.surveyPoints;

            return base.GetInfo() + "\n" +
                "Player Survey Speed: " + StateManager.currentSM.currentSession.SurveySpeed.ToString("N1") + " pts/day" + r; 
        }
    }
}
