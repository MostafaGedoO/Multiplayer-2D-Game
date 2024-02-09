using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Join : MonoBehaviour
{
    public void JoinClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }
}
