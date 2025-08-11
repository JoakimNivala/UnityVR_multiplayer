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
        if (IsServer)
        {
            for (int i = 0; i < arrayObjects.Length; i++)
            {
                var obj = Instantiate(arrayObjects[i], spawnPoints[i].transform.position, Quaternion.identity);
                obj.GetComponent<NetworkObject>().Spawn();
            }
        }
    }

}
