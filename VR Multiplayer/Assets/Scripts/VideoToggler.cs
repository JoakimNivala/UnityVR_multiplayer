using Unity.Netcode;
using UnityEngine;
using UnityEngine.Video;

public class VideoToggler : NetworkBehaviour
{
    private bool isPlaying;
    [SerializeField] VideoPlayer videoPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }


   
}
