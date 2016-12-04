using UnityEngine;
using Hdg;

public class RemoteDebugServerFactory : MonoBehaviour
{
    public void Awake()
    {
        var server = FindObjectOfType<RemoteDebugServer>();
        if (server == null)
        {
            // If there is no server in the scene, then create one.
            gameObject.AddComponent<RemoteDebugServer>();
        }
        else
        {
            // Otherwise destroy ourselves because we aren't needed.
            Destroy(gameObject);
        }
    }
}
