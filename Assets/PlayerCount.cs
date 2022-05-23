using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
public class PlayerCount : NetworkBehaviour
{
    public TMP_Text Number;
    //When connected as server, continuously update player count
    void Update()
    {
        if(IsServer){
            Number.GetComponent<TMP_Text>().text = NetworkManager.Singleton.ConnectedClientsList.Count.ToString();
        }
    }
}
