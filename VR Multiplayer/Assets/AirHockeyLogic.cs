using UnityEngine;
using Unity.Netcode;
using UnityEngine.Video;
using Unity.XR.CoreUtils;
using TMPro;
using System.Collections.Generic;
using System;
public class AirHockeyLogic : NetworkBehaviour
{
    [SerializeField] List<int> m_NetworkScoreVal;
    [SerializeField] private GameObject[]m_Goals;
    [SerializeField] private int playerID;


    private void Awake()
    {
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //When the host starts the local server, it creates the list for scores
        if (IsServer)
        {
            m_NetworkScoreVal = new List<int>
            {
                0,
                0
            };
        
        }
        
    }

    public void ScoreEvent(string name)
    {
        //if the player scores to the player 1's goal, he'll get one point 
        if (name == "Goal")
        {
            Debug.Log(name + ": " + m_NetworkScoreVal[0]);
            //send information to the server about the increment to the score for player 2
            RequestCurrentScoreToServerRpc(1);
        }
        else
        {
            //likewise for player 1
            Debug.Log(name + ": " + m_NetworkScoreVal[1]);
            RequestCurrentScoreToServerRpc(0);
        }
    }

    [Rpc(SendTo.Server)]
    void RequestCurrentScoreToServerRpc(int player, RpcParams rpcParams = default)
    {
        //incerement the score by 1 for whichever player did the goal
        ++m_NetworkScoreVal[player];

        SendCurrentScoreToClientRpc(player, m_NetworkScoreVal[player], RpcTarget.Single(rpcParams.Receive.SenderClientId, RpcTargetUse.Temp));

    }

    [Rpc(SendTo.SpecifiedInParams)]
    void SendCurrentScoreToClientRpc(int player, int val, RpcParams rpcParams = default)
    {
        m_Goals[player].GetComponentInChildren<TextMeshProUGUI>().text = val.ToString();
    }


   

}
