using UnityEngine;

namespace Ships
{
    /// <summary>
    /// An order that can be given to a ship.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// The index of the star system that this order is in. 
        /// </summary>
        public readonly int systemIndex;

        /// <summary>
        /// The system body / jump point that this order at. 
        /// </summary>
        public readonly Point point;

        /// <summary>
        /// Retrieve the point as a local position in the system, in the form of a Vector2 
        /// </summary>
        public Vector2 pointVector2
        {
            get
            {
                return new Vector2(point.CoordX(), point.CoordY());
            }
        }

        /// <summary>
        /// The type of order this order is. (e.g. 0: goto)
        /// </summary>
        public int orderType; 
        
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="_point"></param>
        /// <param name="_orderType"></param>
        /// <param name="_systemIndex"></param>
        public Order(Point _point, int _orderType, int _systemIndex) {
            orderType = _orderType;
            point = _point;
            systemIndex = _systemIndex;
        }

        /// <summary>
        /// Constructor for cloning an order. 
        /// </summary>
        /// <param name="oldOrder">Order to be cloned.</param>
        public Order(Order oldOrder)
        {
            orderType = oldOrder.orderType;
            point = oldOrder.point;
            systemIndex = oldOrder.systemIndex;
        }
    }
}
