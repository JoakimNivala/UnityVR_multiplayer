using UnityEngine;
using Unity.Netcode;

public class SpawnPuck : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject puckSpawnPoint;
    [SerializeField] private GameObject puck;



    public void SpawnPuckCall()
    {
        SpawnNewPuckRPC();
    }


    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SpawnNewPuckRPC(RpcParams rpcParams = default)
    {
        var obj = Instantiate(puck, puckSpawnPoint.transform.position, Quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn();
    }
   
}
