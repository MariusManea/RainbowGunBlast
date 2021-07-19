using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class JoinGame : MonoBehaviour
{
    public NetworkManager manager;

    public float timeout;
    public float waitingTime;

    public int gameCode;

    public GameObject showErrs;


    // Start is called before the first frame update
    void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickJoinGameButton()
    {
        manager.StartMatchMaker();
        manager.matchName = "GameCode: " + gameCode;
        waitingTime = 0;
        manager.matchMaker.ListMatches(0, 20, "GameCode: " + gameCode, false, 0, 0, manager.OnMatchList);

        StartCoroutine(WaitForMatches());
    }

    public IEnumerator WaitForMatches()
    {
        while(manager.matches == null)
        {
            yield return null;
        }



        while (waitingTime < timeout && manager.matches.Count != 0)
        {
            waitingTime += Time.deltaTime;

            var match = manager.matches[Random.Range(0, manager.matches.Count)];
            if (match.currentSize < match.maxSize)
            {
                manager.matchName = match.name;
                manager.matchSize = (uint)match.maxSize;
                manager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, manager.OnMatchJoined);
                break;
            }
        }
        if (waitingTime >= timeout)
        {
            showErrs.GetComponent<Text>().text="Timeout";
        }
        if (manager.matches.Count == 0)
        {
            showErrs.GetComponent<Text>().text="No matches found";
        }
    }
}
