using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SplashAutoDestroy : NetworkBehaviour
{
    public float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 20)
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }
}
