using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalResultsManager : MonoBehaviour
{
    public int kills;
    public int deaths;
    public bool result;
    public Sprite[] results;

    public GameObject killsHandler;
    public GameObject deathsHandler;
    public GameObject resultHandler;


    public void SetResults()
    {
        killsHandler.GetComponent<Text>().text = kills.ToString();
        deathsHandler.GetComponent<Text>().text = deaths.ToString();

        resultHandler.GetComponent<Image>().sprite = results[result ? 1 : 0];
    }

}
