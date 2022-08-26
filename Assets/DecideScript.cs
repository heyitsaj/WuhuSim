using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode.Transports.UNET;
public class DecideScript : NetworkBehaviour
{
    public Canvas UI;
    public Canvas serverUI;
    public Button Host;
    public Button Client;
    public Button Server;
    public Button Quit;
    public Camera cam;
    public TMP_InputField ip;
    
    public TMP_InputField port;
    public TMP_InputField namegiven;
    public TMP_Text errorText;
    public GameObject manager;
    public string username;
    UNetTransport info;
    string ipstr;
    string portstr;
    bool canChange = true;
    public string status;
    UnityEvent connectEvent = new UnityEvent();
    //Compare Strings Input for IP and Port
    string StringCheck(string ip, string port){
        if(ip.Length > 0 && ip.Length - ip.Replace(".","").Length > 0){
            if(port.Length > 0 && int.TryParse(port,out int temp)){
                return "";
            }else{
                return "port";
            }
        }else{
            return "ip";
        }
    }
    //Host button is pressed
    void ChooseHost(){     
        ipstr = ip.GetComponent<TMP_InputField>().text;
        portstr = port.GetComponent<TMP_InputField>().text;
        username = namegiven.GetComponent<TMP_InputField>().text;
        if(status == "connecting"){
            NetworkManager.Singleton.Shutdown();
            status = "";
            errorText.GetComponent<TMP_Text>().color = Color.black;
            errorText.GetComponent<TMP_Text>().text = "";
            Host.GetComponentInChildren<TMP_Text>().text = "Host";
            Client.GetComponentInChildren<TMP_Text>().text = "Client";
            Server.GetComponentInChildren<TMP_Text>().text = "Server";
        }else{
            switch (StringCheck(ipstr,portstr)){
            case "ip":
                errorText.GetComponent<TMP_Text>().color = Color.red;
                errorText.GetComponent<TMP_Text>().text = "Error: Invalid IP";
                break;
            case "port":
                errorText.GetComponent<TMP_Text>().color = Color.red;
                errorText.GetComponent<TMP_Text>().text = "Error: Invalid Port";
                break;
            case "":
                errorText.GetComponent<TMP_Text>().color = Color.black;
                errorText.GetComponent<TMP_Text>().text = "Attempting to Start Server...";
                info.ConnectAddress = ipstr;
                info.ConnectPort = int.Parse(portstr);
                info.ServerListenPort = int.Parse(portstr);
                NetworkManager.Singleton.StartHost();
                status = "connecting";
                Host.GetComponentInChildren<TMP_Text>().text = "Cancel";
                Client.GetComponentInChildren<TMP_Text>().text = "Cancel";
                Server.GetComponentInChildren<TMP_Text>().text = "Cancel";
                break;
            }
        }
        
    }
    //Client button is pressed
    void ChooseClient(){
        ipstr = ip.GetComponent<TMP_InputField>().text;
        portstr = port.GetComponent<TMP_InputField>().text;
        username = namegiven.GetComponent<TMP_InputField>().text;
        if(status == "connecting"){
            NetworkManager.Singleton.Shutdown();
            status = "";
            errorText.GetComponent<TMP_Text>().color = Color.black;
            errorText.GetComponent<TMP_Text>().text = "";
            Host.GetComponentInChildren<TMP_Text>().text = "Host";
            Client.GetComponentInChildren<TMP_Text>().text = "Client";
            Server.GetComponentInChildren<TMP_Text>().text = "Server";
        }
        else{
            switch (StringCheck(ipstr,portstr))
            {
            
            case "ip":
                errorText.GetComponent<TMP_Text>().color = Color.red;
                errorText.GetComponent<TMP_Text>().text = "Error: Invalid IP";
                break;
            case "port":
                errorText.GetComponent<TMP_Text>().color = Color.red;
                errorText.GetComponent<TMP_Text>().text = "Error: Invalid Port";
                break;
            case "":
                errorText.GetComponent<TMP_Text>().color = Color.black;
                errorText.GetComponent<TMP_Text>().text = "Connecting...";
                info.ConnectAddress = ipstr;
                info.ConnectPort = int.Parse(portstr);
                info.ServerListenPort = int.Parse(portstr);
                NetworkManager.Singleton.StartClient();
                status = "connecting";
                Host.GetComponentInChildren<TMP_Text>().text = "Cancel";
                Client.GetComponentInChildren<TMP_Text>().text = "Cancel";
                Server.GetComponentInChildren<TMP_Text>().text = "Cancel";
                break;
            }
        }
    }
    //Server button is pressed
    void ChooseServer(){
        ipstr = ip.GetComponent<TMP_InputField>().text;
        portstr = port.GetComponent<TMP_InputField>().text;
        username = namegiven.GetComponent<TMP_InputField>().text;
        if(status == "connecting"){
            NetworkManager.Singleton.Shutdown();
            status = "";
            errorText.GetComponent<TMP_Text>().color = Color.black;
            errorText.GetComponent<TMP_Text>().text = "";
            Host.GetComponentInChildren<TMP_Text>().text = "Host";
            Client.GetComponentInChildren<TMP_Text>().text = "Client";
            Server.GetComponentInChildren<TMP_Text>().text = "Server";
        }else{
            switch(StringCheck(ipstr,portstr)){
            case "ip":
                errorText.GetComponent<TMP_Text>().color = Color.red;
                errorText.GetComponent<TMP_Text>().text = "Error: Invalid IP";
                break;
            case "port":
                errorText.GetComponent<TMP_Text>().color = Color.red;
                errorText.GetComponent<TMP_Text>().text = "Error: Invalid Port";
                break;
            case "":
                errorText.GetComponent<TMP_Text>().color = Color.black;
                errorText.GetComponent<TMP_Text>().text = "Attempting to Start Server...";
                info.ConnectAddress = ipstr;
                info.ConnectPort = int.Parse(portstr);
                info.ServerListenPort = int.Parse(portstr);
                
                NetworkManager.Singleton.StartServer();
                status = "connecting";
                Host.GetComponentInChildren<TMP_Text>().text = "Cancel";
                Client.GetComponentInChildren<TMP_Text>().text = "Cancel";
                Server.GetComponentInChildren<TMP_Text>().text = "Cancel";
                break;
            }
        }
        
    }
    //If connected, execute once when connected
    void ConnectionMade(){
        if (canChange){
            canChange = false;
            status = "connected";
            errorText.GetComponent<TMP_Text>().text = "";
            Host.GetComponentInChildren<TMP_Text>().text = "Host";
            Client.GetComponentInChildren<TMP_Text>().text = "Client";
            Server.GetComponentInChildren<TMP_Text>().text = "Server";
            if(IsClient||IsHost){
                cam.gameObject.SetActive(false);
                serverUI.gameObject.SetActive(false);
            }else if(IsServer){
                serverUI.gameObject.SetActive(true);
            }
            UI.gameObject.SetActive(false);
        }
    }
    void ChooseQuit(){
        Application.Quit();
    }
    public void ExitServer(){
        status = "";
        NetworkManager.Shutdown();
        cam.gameObject.SetActive(true);
        UI.gameObject.SetActive(true);
        serverUI.gameObject.SetActive(false);
    }
    //Establish listeners
    void Start(){
        info = manager.GetComponent<UNetTransport>();
        Host.onClick.AddListener(ChooseHost);
        Client.onClick.AddListener(ChooseClient);
        Server.onClick.AddListener(ChooseServer);
        Quit.onClick.AddListener(ChooseQuit);
        connectEvent.AddListener(ConnectionMade);
        
    }
    //Check if connected: if not, reset switch
    void Update(){
        if(IsClient || IsHost || IsServer){
            connectEvent.Invoke();

        }else{
            canChange = true;
        }
    }
}
