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


   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();      
    }

    public void ScoreEvent(string name)
    {
        //if the player 2 scores to the player 1's goal, he'll get one point 
        if (name == "Goal")
        {
           
            RequestCurrentScoreToServerRpc(1);
        }
        else
        {
            //likewise for player 1
           
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
