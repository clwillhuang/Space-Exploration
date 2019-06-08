using Space;
using UnityEngine;
using UnityEngine.UI; 

/// <summary>
/// Show a jump point icon on the screen.
/// </summary>
public class JumpPointGO : MonoBehaviour {

    /// <summary>
    /// The jump point this gameobject represents.
    /// </summary>
    private JumpPoint _jumpPoint;

    /// <summary>
    /// The jump point this gameobject represents.
    /// </summary>
    public JumpPoint jumpPoint
    {
        get
        {
            return _jumpPoint;
        }
        set
        {
            if (value != null)
            {
                _jumpPoint = value;
            }
        }
    }

    /// <summary>
    /// The sprite for an unsurveyed jump point. 
    /// </summary>
    [SerializeField]
    private Sprite UnsurveyedPoint;

    /// <summary>
    /// The sprite for an connecting jump point. 
    /// </summary>
    [SerializeField]
    private Sprite ConnectedPoint;

    public void OnEnable()
    {
        GetComponentInChildren<Text>().text = "";
        if (_jumpPoint != null)
        {
            if (_jumpPoint.isSurveyed)
                GetComponentInChildren<SpriteRenderer>().sprite = ConnectedPoint;
            else
                GetComponentInChildren<SpriteRenderer>().sprite = UnsurveyedPoint;
        }
    }

    /// <summary>
    /// When mouse enters box collider
    /// </summary>
    public void OnMouseEnter()
    {
        GetComponentInChildren<Text>().text = jumpPoint.LocationName;
    }

    /// <summary>
    /// When mouse exits box collider
    /// </summary>
    public void OnMouseExit()
    {
        if (!jumpPoint.IsValidConnection())
            GetComponentInChildren<Text>().text = "";
    }

    /// <summary>
    /// Update the labelling and sprite to match the jump point. 
    /// </summary>
    public void FixedUpdate()
    {
        if (_jumpPoint.isSurveyed)
        {
            GetComponentInChildren<SpriteRenderer>().sprite = ConnectedPoint;
            if (_jumpPoint.IsValidConnection())
                GetComponentInChildren<Text>().text = jumpPoint.LocationName;
            else
                Destroy(gameObject);
        }
        else
            GetComponentInChildren<SpriteRenderer>().sprite = UnsurveyedPoint;
    }

    /// <summary>
    /// When this point is clicked.
    /// </summary>
    public void OnMouseDown()
    {
        if (_jumpPoint.isSurveyed && _jumpPoint.IsValidConnection())
            UIManager.current.ShowSystem(_jumpPoint.connectingSystem);
        else
            _jumpPoint.Survey();
    }
}
