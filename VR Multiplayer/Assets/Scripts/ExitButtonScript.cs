using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;

public class ExitButtonScript : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void ExitButton()
    {
        XRINetworkGameManager.Instance.Disconnect();
    }
}
