using UnityEngine;
using Unity.Netcode;
using UnityEngine.Video;
using Unity.XR.CoreUtils;
using TMPro;
public class AirHockeyLogic : NetworkBehaviour
{
    private int[] m_ScoreValue;
    [SerializeField] NetworkVariable<int>[] m_NetworkScoreVal;
   [SerializeField] private GameObject[]m_Goals;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

    }

    public void ScoreEvent(string name)
    {
        //if the player scores to the player 1's goal, he'll get one point 
        if (name == "Goal")
        {
            Debug.Log(name);
            //send information to the server about the increment to the score for player 2
            RequestCurrentScoreToServerRpc(1);
        }
    }

    [Rpc(SendTo.Server)]
    void RequestCurrentScoreToServerRpc(int player, RpcParams rpcParams = default)
    {
        //increase the second player's point 
        ++m_ScoreValue[player];

        SendCurrentScoreToClientRpc((int)player, m_ScoreValue[player], RpcTarget.Single(rpcParams.Receive.SenderClientId, RpcTargetUse.Temp));

    }

    [Rpc(SendTo.Everyone)]
    void SendCurrentScoreToClientRpc(int player, int val, RpcParams rpcParams = default)
    {
        m_Goals[player].GetComponentInChildren<TextMeshProUGUI>().text = m_ScoreValue[val].ToString();
    }


   

}
