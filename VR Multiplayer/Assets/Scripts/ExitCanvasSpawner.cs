using UnityEngine;
using Unity.Netcode;
using XRMultiplayer;

public class ExitCanvasSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject ExitCanvas;
 

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
      
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
       
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created :3

}
