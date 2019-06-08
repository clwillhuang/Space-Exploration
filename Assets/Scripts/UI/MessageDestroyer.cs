using UnityEngine;

/// <summary>
/// Class attached to each generated message, controlling when it is destroyed.
/// </summary>
public class MessageDestroyer : MonoBehaviour {

    public void OnEnable()
    {
        GetComponent<Animation>().Play("MessageAppear");
        Invoke("DestroyMessage", Constants.MESSAGE_VISIBLE); 
    }

    /// <summary>
    /// Initiate the sequence to fade away and destroy message.
    /// </summary>
    public void DestroyMessage()
    {
        GetComponent<Animation>().Play("MessageFade");
        Destroy(gameObject, 4.5f);
    }
}
