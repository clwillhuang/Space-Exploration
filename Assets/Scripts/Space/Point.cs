
/// <summary>
/// Base class for any object or location that exists in space.
/// </summary>
public abstract class Point {

    /// <summary>
    /// The x-coordinate of this object / point in the system.
    /// </summary>
    /// <returns>X coordinate</returns>
    public abstract float CoordX();

    /// <summary>
    /// The y-coordinate of this object / point in the system.
    /// </summary>
    /// <returns>Y coordinate</returns>
    public abstract float CoordY(); 

    /// <summary>
    /// The name of this location. 
    /// </summary>
    public string LocationName { get; protected set; }

    /// <summary>
    /// Survey this object / jump point.
    /// </summary>
    public abstract void Survey();

    /// <summary>
    /// Has this point been surveyed?
    /// </summary>
    public bool isSurveyed { get; protected set; }

    /// <summary>
    /// The survey points required to survey this jump point / planet. 
    /// </summary>
    public float surveyPoints { get; set; }  

    /// <summary>
    /// Constructor for a point. 
    /// </summary>
    /// <param name="_locationName">The name of this location.</param>
    /// <param name="_surveyPoints">Survey points required to survey this object or point.</param>
    public Point(string _locationName, float _surveyPoints)
    {
        surveyPoints = _surveyPoints;
        LocationName = _locationName; 
    }

}
