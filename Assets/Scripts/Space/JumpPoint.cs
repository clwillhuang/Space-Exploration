using UnityEngine; 

namespace Space
{
    /// <summary>
    /// A class for a jump point which links two systems together.
    /// </summary>
    public class JumpPoint : Point
    {
        /// <summary>
        /// X coordinate in system
        /// </summary>
        private readonly float x;

        /// <summary>
        /// Y coordinate in system
        /// </summary>
        private readonly float y;

        /// <summary>
        /// The x-coordinate of this object in the system.
        /// </summary>
        /// <returns>X coordinate</returns>
        public override float CoordX()
        {
            return x;
        }

        /// <summary>
        /// The y-coordinate of this object in the system.
        /// </summary>
        /// <returns>Y coordinate</returns>
        public override float CoordY()
        {
            return y;
        }

        /// <summary>
        /// The system that this jump point connects to. 
        /// </summary>
        public int connectingSystem { get; set; }

        /// <summary>
        /// Index reference to the system that this jump point is in.
        /// </summary>
        private int systemLocation;

        /// <summary>
        /// The index of this jump point within the system.
        /// </summary>
        public readonly int jumpPointIndex;

        /// <summary>
        /// The corresponding jump point that it connects to. 
        /// </summary>
        public int connectingJumpPoint { get; private set; }

        /// <summary>
        /// Set the jump point that this jump point connects to.
        /// </summary>
        /// <param name="jp"></param>
        public void SetConnectingJumpPoint(int jp)
        {
            connectingJumpPoint = jp;
        }

        /// <summary>
        /// Does this jump point connect to another system? 
        /// </summary>
        public bool IsValidConnection()
        {
            return connectingSystem != -1; 
        }

        /// <summary>
        /// Has it been pre-determined that this jump point will lead into a new system?
        /// </summary>
        public readonly bool willbeConnection;

        /// <summary>
        /// Generate a new system which connects to an existing system. 
        /// </summary>
        /// <param name="_systemLocation">The new index for this new system, in Player.galaxy</param>
        /// <param name="_willBeConnection">Will jump point be guaranteed to connect to new system?</param>
        public JumpPoint(int _systemLocation, int _jumpPointIndex, bool _willBeConnection=false) : base("Jump Point #" + (_jumpPointIndex + 1) + " (UNSURVEYED)", Constants.JUMP_POINT_SURVEY_POINTS)
        {
            // Randomize location in system.
            x = Random.Range(-Constants.MAX_COORDINATE_X, Constants.MAX_COORDINATE_X);
            y = Random.Range(-Constants.MAX_COORDINATE_Y, Constants.MAX_COORDINATE_Y);

            willbeConnection = _willBeConnection;

            connectingSystem = -1;
            connectingJumpPoint = -1; 
            systemLocation = _systemLocation;
            jumpPointIndex = _jumpPointIndex; 
        }

        /// <summary>
        /// Generate a new system which connects to an existing system. 
        /// </summary>
        /// <param name="_systemLocation">The new index for this new system, in Player.galaxy</param>
        /// <param name="_jumpPointIndex">The new index for this new jump point.</param>
        /// <param name="oldSystem">The existing system that the new system will connect to.</param>
        /// <param name="jp">The existing jump point that the new system will connect via.</param>
        public JumpPoint(int _systemLocation, int _jumpPointIndex, int oldSystem, int jp) : base("Jump Point #" + (_jumpPointIndex + 1) + " (UNSURVEYED)", Constants.JUMP_POINT_SURVEY_POINTS)
        {
            // Randomize location in system.
            x = Random.Range(-Constants.MAX_COORDINATE_X, Constants.MAX_COORDINATE_X);
            y = Random.Range(-Constants.MAX_COORDINATE_Y, Constants.MAX_COORDINATE_Y);

            // Connect this jump point to existing entities.
            connectingSystem = oldSystem;
            connectingJumpPoint = jp;
            LocationName = "Jump Point #" + (_jumpPointIndex + 1) + " to " + StateManager.currentSM.currentSession.galaxy[oldSystem].SystemName; 

            // Since already connects to something, no survey is required.
            isSurveyed = true; 

            // Connect existing entities to this jump point
            StateManager.currentSM.currentSession.galaxy[oldSystem].JumpPoints[jp].SetConnectingJumpPoint(jumpPointIndex);
            systemLocation = _systemLocation;

            jumpPointIndex = _jumpPointIndex;

        }

        /// <summary>
        /// Survey this jump point, determine whether or not this jump point connects to new system.
        /// </summary>
        public override void Survey()
        {
            Debug.Log("Surveying");

            // Avoid surveying already surveyed points.
            if (isSurveyed) return;

            // Will this connect to new system? 
            bool generateNewSystem = willbeConnection || Random.Range(0f, 1f) <= Constants.NEW_SYSTEM_GENERATION_CHANCE;

            // Generate new system
            if (generateNewSystem)
            {
                UIManager.current.DisplayMessage("Jump point surveyed in " + StateManager.currentSM.currentSession.galaxy[systemLocation].SystemName + ". New system was found.");
                StateManager.currentSM.currentSession.GenerateNewSystem(StateManager.currentSM.currentSession.galaxy[systemLocation],
                    this);
            }
            // Will not connect to new system
            else
            {
                connectingSystem = -1;
                UIManager.current.DisplayMessage("Jump point surveyed in " + StateManager.currentSM.currentSession.galaxy[systemLocation].SystemName + ". No new system was discovered.");
            }

            if (IsValidConnection())
                LocationName = "Jump Point #" + (jumpPointIndex + 1) + " to " + StateManager.currentSM.currentSession.galaxy[connectingSystem].SystemName;
            else
                LocationName = "Jump Point #" + (jumpPointIndex + 1) + " (empty)";


            isSurveyed = true; 
        }

    }

}