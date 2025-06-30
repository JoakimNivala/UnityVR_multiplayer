using Unity.Netcode;
using UnityEngine;

public class SpawnTable : NetworkBehaviour
{
    [SerializeField] private GameObject table;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

    }

    [ServerRpc]
    public void SpawnServerRpc()
        //server RPC methods require the ServerRpc suffix to the end of the method or unity will throw error.
    {
        var obj = Instantiate(table, new Vector3(-4.607f, 8.932f, -20.05589f), Quaternion.Euler(0, -66.435f, 0));
        obj.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        
    }
}
