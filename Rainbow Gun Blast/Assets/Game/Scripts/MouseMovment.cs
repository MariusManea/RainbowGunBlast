using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MouseMovment : NetworkBehaviour
{
	public GameObject spine;
	public Transform target;
	public Transform upLimit;
	public Transform downLimit;
    // Mouse direction.
	private Vector2 mDir;

	// Parent body (e.g. a capsule).
	// This script should be attached to the
	// Main Camera, and the Main Camera should
	// be a child object of a capsule.
	private Transform myBody;
	public GameObject player;

	public GameObject controls;

	void Start () {
		if (this.transform.parent.GetComponent<NetworkIdentity>().isLocalPlayer)
		{
			controls = GameObject.Find("InGameManager");
			this.gameObject.SetActive(true);
		} else
		{
			this.gameObject.SetActive(false);
		}
		// Grab the capsule body's transform.
		// This is so that we can rotate the
		// main body of the character.
		myBody = this.transform.parent.transform;
	}
	

	void Update () {
		if (!this.transform.parent.GetComponent<NetworkIdentity>().isLocalPlayer) { return; }
		if (this.transform.parent.GetComponent<walk>().finito) { return; }

		// How much has mouse moved across screen?
		Vector2 mc = new Vector2(Input.GetAxisRaw("Mouse X"),
			Input.GetAxisRaw("Mouse Y"));

		// Add new movement to current mouse direction.
		mDir += mc;

		// Rotate head up or down.
		// This rotates the camera on X-axis.

		if (Mathf.Sign(mc.y) < 0)
		{
			if (Mathf.Abs(Vector3.Distance(this.transform.localPosition, upLimit.localPosition)) > 0.01f)
			{
				this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, upLimit.localPosition, -mc.y * controls.GetComponent<OptionsWindow>().conf.sensitivity * Time.deltaTime);
			}
		}
		else
		{
			if (Mathf.Abs(Vector3.Distance(this.transform.localPosition, downLimit.localPosition)) > 0.01f)
			{
				this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, downLimit.localPosition, mc.y * controls.GetComponent<OptionsWindow>().conf.sensitivity * Time.deltaTime);
			}
		}

		// Rotate body left or right.
		// This rotates the parent body (a capsule), not the camera,
		// on the Y-axis.
		if (!player.GetComponent<walk>().isDead && Time.timeScale != 0)
		{
			myBody.localRotation =
				Quaternion.AngleAxis(mDir.x * controls.GetComponent<OptionsWindow>().conf.sensitivity / 5, Vector3.up);
		}

		this.transform.LookAt(target);
	}

	void LateUpdate()
	{
		if (!this.transform.parent.GetComponent<NetworkIdentity>().isLocalPlayer) { return; }
		if (this.transform.parent.GetComponent<walk>().finito) { return; }
		if (this.transform.parent.GetComponent<WeaponLogic>().isAiming)
		{
			spine.transform.eulerAngles = new Vector3(spine.transform.eulerAngles.x, spine.transform.eulerAngles.y, -this.transform.eulerAngles.x - 80);
		}
	}
}
