using UnityEngine;
using Unity.Netcode;

public class GoalScript : NetworkBehaviour
{
    private AirHockeyLogic airHockeyLogic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

    }

    private void Start()
    {
        airHockeyLogic = GetComponentInParent<AirHockeyLogic>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("goal");
        if (other.CompareTag("Goal"))
        {
            airHockeyLogic.ScoreEvent(gameObject.name);
            
        }
    }
}
