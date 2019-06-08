using Space;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Show a system body on screen.
/// </summary>
public class SystemBodyGO : MonoBehaviour {

    /// <summary>
    /// The system body reference for this celestial object
    /// </summary>
    [SerializeField]
    private SystemBody _systemBody;

    /// <summary>
    /// The parent transform containing all celestial objects orbiting this object
    /// </summary>
    public Transform orbiting;

    /// <summary>
    /// The system body reference for this celestial object
    /// </summary>
    public SystemBody systemBody
    {
        get
        {
            return _systemBody; 
        }
        set
        {
            _systemBody = value;
            GetComponentInChildren<SpriteRenderer>().sprite = _systemBody.sprite;
            if (value is OrbitingBody)
            {
                float scale = Mathf.Clamp(_systemBody.mass / 100f + 0.5f, 0.1f, 0.5f) * 10f;
                GetComponentInChildren<SpriteRenderer>().transform.localScale = new Vector3(scale, scale, scale); 
            }
            GetComponentInChildren<Text>().text = "";
            GetComponent<BoxCollider2D>().size = new Vector2(30f, 30f);

        }
    }

    /// <summary>
    /// When mouse enters box collider
    /// </summary>
    public void OnMouseEnter()
    {
        if (_systemBody is OrbitingBody)
        {
            GetComponentInChildren<Text>().text = _systemBody.LocationName +
                ((_systemBody as OrbitingBody).colony != null ? " (C)" : "") +
                ((_systemBody as OrbitingBody).isHabitable ? " (H) " : "");
        }
        else GetComponentInChildren<Text>().text = _systemBody.LocationName;
    }

    /// <summary>
    /// When mouse exits box collider
    /// </summary>
    public void OnMouseExit()
    {
        GetComponentInChildren<Text>().text = "";
    }

    /// <summary>
    /// When box collider is clicked
    /// </summary>
    public void OnMouseDown()
    {
        // Only allow the establishment of a colony on a non-star entity
        if (_systemBody is OrbitingBody)
        {
            if ((_systemBody as OrbitingBody).colony == null)
                (_systemBody as OrbitingBody).EstablishColony(); 
        }
        else if (_systemBody is Star)
        {
            UIManager.current.DisplayMessage("Colonies cannot be established on stars.");
        }
    }

    public void Update()
    {
        if (systemBody == null)
        {
            return; 
        }
        if (systemBody is OrbitingBody)
        {
            transform.localPosition = new Vector3((systemBody as OrbitingBody).CoordX(), (systemBody as OrbitingBody).CoordY(), 0f);
        }
    }
}
