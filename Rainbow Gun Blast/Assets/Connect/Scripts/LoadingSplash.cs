using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSplash : MonoBehaviour
{
    public GameObject[] Loads;
    public bool isLoading;
    public float time;
    public int state;

    // Start is called before the first frame update
    void Start()
    {
        time = Time.time;
        state = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLoading)
        {
            if (Time.time - time > 1)
            {
                time = Time.time;
                if (state > 2)
                {
                    ResetLoads();
                }
                Loads[state++].SetActive(true);
                
            }
        } 
        else
        {
            ResetLoads();
        }
    }

    private void ResetLoads()
    {
        state = 0;
        foreach(GameObject load in Loads)
        {
            load.SetActive(false);
        }
    }
}
