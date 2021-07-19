using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletLogic : NetworkBehaviour
{
	public GameObject splash;
    public Transform spawnedSplash;
    public Material[] mat;
    RaycastHit[] hits;
    public NetworkConnection netCon;
    public float timer;
    public GameObject owner;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        hits = Physics.RaycastAll(new Ray(this.transform.position, this.GetComponent<Rigidbody>().velocity.normalized), 1);

        if (hits.Length > 0) {


            CmdSpawnSplash();
            splash.GetComponent<MeshRenderer>().material=mat[Random.Range(0, 5)];
            
            NetworkServer.Destroy(this.gameObject);

        }
        if (timer > 10)
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }

    [Command]
    public void CmdSpawnSplash()
    {
        if (hits.Length == 0) { return; }

        if (hits[0].transform.tag == "Enemy")
        {
            hits[0].transform.parent.GetComponent<walk>().CmdTakeDamage(GameObject.Find("PlayerDistribution").GetComponent<GameManagerBehaviour>().startMatch ? 20 : 0, owner);
        }

        Quaternion rot = Quaternion.FromToRotation(Vector3.up, hits[0].normal);
        Vector3 pos = hits[0].point;
        spawnedSplash = GameObject.Instantiate(splash.transform, pos, rot, hits[0].transform);
        spawnedSplash.SetParent(hits[0].transform);
        NetworkServer.SpawnWithClientAuthority(spawnedSplash.gameObject, netCon);
    }
}
