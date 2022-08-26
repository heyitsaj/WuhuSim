using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ExitHandler : NetworkBehaviour
{
    public GameObject manager;
    public Camera menucam;
    public Canvas UI;
    public Canvas serverStats;
    public GameObject decideScript;
    public TMP_Text errorText;
    // Update is called once per frame
    void Update(){
        //Disconnect from server
        if(!(IsClient || IsServer || IsHost) && decideScript.GetComponent<DecideScript>().status == "connected"){
            decideScript.GetComponent<DecideScript>().status = "";
            menucam.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            UI.gameObject.SetActive(true);
            errorText.GetComponent<TMP_Text>().color = Color.red;
            errorText.GetComponent<TMP_Text>().text = "Error: Disconnected From Server";
        }
        //Escape key pressed, Open Menu
        if (Input.GetKeyDown(KeyCode.Escape) && !(IsClient || IsHost)){
            serverStats.gameObject.SetActive(false);
            decideScript.GetComponent<DecideScript>().status = "";
            NetworkManager.Shutdown();
            menucam.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            UI.gameObject.SetActive(true);
        }
    }
}
