using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour
{
    public NetworkManager manager;
    public int gameCode;
    // Start is called before the first frame update
    void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickHostGameButton()
    {
        manager.StartMatchMaker();
        manager.matchName = "GameCode: " + gameCode;
        manager.matchSize = 10;
        manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", "", "", 0, 0, manager.OnMatchCreate);
    }
}
