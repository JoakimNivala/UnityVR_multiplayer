using UnityEngine;
using Unity.Netcode;
public class NetworkObjectDestroyerMe : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Puck" || other.gameObject.tag == "Thing" || other.gameObject.tag == "Table")
        {
            if (IsServer)
            {
                Debug.Log("LO");
                var obj = other.transform.root.GetComponent<NetworkObject>();
                obj.Despawn(destroy: true);
               
            }
        }
             
    }
}
