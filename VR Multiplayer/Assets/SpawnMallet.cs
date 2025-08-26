using UnityEngine;
using Unity.Netcode;

public class SpawnMallet : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject malletSpawnPoint;
    [SerializeField] private GameObject mallet;



    public void SpawnMalletCall()
    {
        SpawnNewMalletRPC();
    }


    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SpawnNewMalletRPC(RpcParams rpcParams = default)
    {
        var obj = Instantiate(mallet, malletSpawnPoint.transform.position, Quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn();
    }

}
