using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    // 100 = Solo - Deathmatch | 101 = Solo - Last Man Standing
    // 110 = Team - Deathmatch | 111 = Team - Last Man Standing
    public int gameCode;
    public GameObject manager;

    void Awake()
    {
        manager = GameObject.Find("NetworkManager");
    }
    void Update()
    {
        manager.GetComponent<HostGame>().gameCode = gameCode;
        manager.GetComponent<JoinGame>().gameCode = gameCode;
    }
}
