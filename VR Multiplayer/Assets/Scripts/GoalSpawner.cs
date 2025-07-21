using UnityEngine;
using Unity.Netcode;

public class GoalSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject[] arrayObjects;
    [SerializeField] private GameObject[] spawnPoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GoalLogicStarterRpc();
        
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void GoalLogicStarterRpc(RpcParams rpcParams = default)
    {
        for (int i = 0; i < arrayObjects.Length; i++)
        {
            var obj = Instantiate(arrayObjects[i], spawnPoints[i].transform.position, Quaternion.identity);
            obj.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        }
    }
   
}
