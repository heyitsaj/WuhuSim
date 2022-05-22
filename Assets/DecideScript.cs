using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode.Transports.UNET;
public class DecideScript : MonoBehaviour
{
    public Canvas UI;
    public Button Host;
    public Button Client;
    public Button Server;
    public Camera cam;
    public TMP_InputField ip;
    
    public TMP_InputField port;
    public TMP_InputField namegiven;
    public GameObject manager;
    public string username;
    UNetTransport info;
    string ipstr;
    string portstr;
    void ChooseHost(){     
        ipstr = ip.GetComponent<TMP_InputField>().text;
        portstr = port.GetComponent<TMP_InputField>().text;
        username = namegiven.GetComponent<TMP_InputField>().text;
        info.ConnectAddress = ipstr;
        info.ConnectPort = int.Parse(portstr);
        info.ServerListenPort = int.Parse(portstr);
        cam.gameObject.SetActive(false);
        UI.enabled = false;
        NetworkManager.Singleton.StartHost();
    }
    void ChooseClient(){
        ipstr = ip.GetComponent<TMP_InputField>().text;
        portstr = port.GetComponent<TMP_InputField>().text;
        username = namegiven.GetComponent<TMP_InputField>().text;
        info.ConnectAddress = ipstr;
        info.ConnectPort = int.Parse(portstr);
        info.ServerListenPort = int.Parse(portstr);
        cam.gameObject.SetActive(false);
        UI.enabled = false;
        NetworkManager.Singleton.StartClient();
    }
    void ChooseServer(){
        ipstr = ip.GetComponent<TMP_InputField>().text;
        portstr = port.GetComponent<TMP_InputField>().text;
        username = namegiven.GetComponent<TMP_InputField>().text;
        info.ConnectAddress = ipstr;
        info.ConnectPort = int.Parse(portstr);
        info.ServerListenPort = int.Parse(portstr);
        UI.enabled = false;
        NetworkManager.Singleton.StartServer();
    }

    void Start(){
        info = manager.GetComponent<UNetTransport>();
        Host.onClick.AddListener(ChooseHost);
        Client.onClick.AddListener(ChooseClient);
        Server.onClick.AddListener(ChooseServer);
        
    }
}
