using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponLogic : NetworkBehaviour
{
	public bool isAiming;
	public Rigidbody projectile;
	public float speed;

	private Transform bulletSpawnPointTrans;

    public Transform body;
    public Quaternion normalOrientation;
    public Quaternion aimingOrientation;


	void Awake() {
        bulletSpawnPointTrans = this.GetComponent<PlayerController>().bulletSpawnPoint.transform;
        aimingOrientation = new Quaternion(body.localRotation.x, body.localRotation.y + 0.12f, body.localRotation.z, body.localRotation.w);
        normalOrientation = new Quaternion(body.localRotation.x, body.localRotation.y, body.localRotation.z, body.localRotation.w);


	}

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) { return; }
        if (GetComponent<walk>().finito) { return; }
        if (GetComponent<walk>().isDead) { return; }
        if (Input.GetMouseButton(1)) {
        	isAiming = true;
    		this.GetComponent<Actions>().Aiming();

            body.localRotation = aimingOrientation;

    		if (Input.GetMouseButtonDown(0)) {
                CmdSpawnBullet(bulletSpawnPointTrans.position, bulletSpawnPointTrans.rotation, bulletSpawnPointTrans.TransformDirection(new Vector3(0, 0, speed)));
    		}
    	} else {
    		isAiming = false;
            normalOrientation = new Quaternion(body.localRotation.x, body.localRotation.y, body.localRotation.z, body.localRotation.w);
            body.localRotation = normalOrientation;
    	}
    }

    [Command]
    public void CmdSpawnBullet(Vector3 position, Quaternion rotation, Vector3 direction)
    {
        Rigidbody instantiateProjectile = Instantiate(projectile,
                                            position,
                                            rotation
                                            ) as Rigidbody;
        instantiateProjectile.gameObject.GetComponent<BulletLogic>().netCon = connectionToClient;
        instantiateProjectile.velocity = direction;
        instantiateProjectile.gameObject.GetComponent<BulletLogic>().owner = this.gameObject;
        NetworkServer.SpawnWithClientAuthority(instantiateProjectile.gameObject, connectionToClient);
        
    }
}
