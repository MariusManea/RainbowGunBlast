using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class walk : NetworkBehaviour
{
	public float runningSpeed;
	public float walkingSpeed;
	public float speed;

	public float movement;
	public float sideStep;

	public GameObject resultsWindow;

	[SyncVar]
	public float deathTime;
	public bool isRunning;
	public GameObject character;
	public CharacterController characterController;
	public Transform body;
	public Quaternion normalOrientation;

	public GameObject controls;

	public Material[] matTeams;

	public Vector3 respawnPosition;

	[SyncVar(hook = "OnChangeHealth")]
	public float health;
	[SyncVar]
	public bool isDead;

	[SyncVar]
	public int score;
	[SyncVar]
	public int deaths;

	public bool finito;

	private void Awake()
	{
		isRunning = true;
		characterController = this.GetComponent<CharacterController>();
		normalOrientation = new Quaternion(body.localRotation.x, body.localRotation.y, body.localRotation.z, body.localRotation.w);
		controls = GameObject.Find("InGameManager");
	}

	void Update () {
		if (!isLocalPlayer) { return; }
		if (finito) { return; }
		if (isDead)
		{
			return;
		}

		if (this.GetComponent<WeaponLogic>().isAiming == false) {

			if (Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), controls.GetComponent<OptionsWindow>().conf.forwards)))
			{
				movement = 1;
			} else
			{
				if (Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), controls.GetComponent<OptionsWindow>().conf.backwards)))
				{
					movement = -1;
				}
			}

			//movement = Input.GetAxis ("Vertical");
			movement *= Time.deltaTime;

			if (Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), controls.GetComponent<OptionsWindow>().conf.right)))
			{
				sideStep = 1;
			}
			else
			{
				if (Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), controls.GetComponent<OptionsWindow>().conf.left)))
				{
					sideStep = -1;
				}
			}
			//sideStep = Input.GetAxis ("Horizontal");
			sideStep *= Time.deltaTime;

			if (Input.GetKey(KeyCode.LeftShift) || Mathf.Sign(movement) < 0)
			{
				speed = Mathf.Lerp(speed, walkingSpeed, 10 * Time.deltaTime);
				isRunning = false;
			}
			else
			{
				speed = Mathf.Lerp(speed, runningSpeed, 3 * Time.deltaTime);
				isRunning = true;
			}

			characterController.Move(this.transform.forward * movement * speed);
			characterController.Move(this.transform.right * sideStep * speed);
			if (!characterController.isGrounded)
			{
				characterController.Move(this.transform.up * -9.81f * Time.deltaTime);
			}

			if (Mathf.Abs(sideStep) < 0.0025f)
			{
				body.localRotation = Quaternion.Lerp(body.localRotation, normalOrientation, 10f * Time.deltaTime);
			} else
			{
				if (Mathf.Abs(movement) < 0.0025f)
				{
					body.localRotation = Quaternion.Lerp(body.localRotation, normalOrientation * new Quaternion(0, Mathf.Sign(sideStep) * Mathf.Sign(movement), 0, 1), 5f * Time.deltaTime);
				} else
				{
					body.localRotation = Quaternion.Lerp(body.localRotation, normalOrientation * new Quaternion(0, Mathf.Sign(sideStep) * Mathf.Sign(movement) / 3, 0, 1), 5f * Time.deltaTime);
				}
			}

			if (movement != 0 || sideStep != 0) {
				if (isRunning && Mathf.Sign(movement) > 0) {
					this.GetComponent<Actions>().Run();
				} else {
					this.GetComponent<Actions>().Walk();
				}
			} else {
				speed = 0;
				this.GetComponent<Actions>().Stay();
			}
		} else {
			body.localRotation = normalOrientation;
		}
	}

	[ClientRpc]
	public void RpcChangeMaterial(int index)
	{
		transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = matTeams[index];
	}

	[Command]
	public void CmdTakeDamage(float damage, GameObject owner)
	{
		if (!finito)
		{
			health -= damage;
			if (health <= 0)
			{
				owner.GetComponent<walk>().CmdScore();
			}
		}
	}
	
	void OnChangeHealth(float health)
	{
		if (health <= 0 && !isDead)
		{
			deathTime = 0;
			deaths++;
			isDead = true;

			this.transform.GetChild(2).gameObject.SetActive(false);
			GetComponent<CharacterController>().enabled = false;
			GetComponent<Actions>().Death();
		}
	}

	[Command]
	public void CmdScore()
	{
		if (!finito)
		{
			score++;
		}
	}

	void Respawn(bool warmUp, Vector3 position)
	{
		if (warmUp)
		{
			transform.GetChild(2).gameObject.SetActive(false);
			GetComponent<CharacterController>().enabled = false;
			transform.position = position;
			GetComponent<Animator>().Rebind();
			transform.GetChild(2).gameObject.SetActive(true);
			GetComponent<CharacterController>().enabled = true;
			GetComponent<walk>().health = 100;
			GetComponent<walk>().isDead = false;
		}
		else
		{
			deathTime += Time.deltaTime;
			if (deathTime > 5)
			{
				transform.position = position;
				GetComponent<Animator>().Rebind();
				transform.GetChild(2).gameObject.SetActive(true);
				GetComponent<CharacterController>().enabled = true;
				GetComponent<walk>().health = 100;
				GetComponent<walk>().isDead = false;
			}
		}
	}

	[Command]
	public void CmdRespawn(GameObject caller, bool warmUp, Vector3 position)
	{
		Respawn(warmUp, position);
		RpcRespawn(caller, warmUp, position);
	}

	[ClientRpc]
	public void RpcRespawn(GameObject caller, bool warmUp, Vector3 position)
	{
		if (isServer) { return; }
		if (caller == this.gameObject) { return; }

		Respawn(warmUp, position);
	}

	[ClientRpc]
	public void RpcSetRespawnPoint(Vector3 position)
	{
		respawnPosition = position;
	}

	void SetResults(bool result)
	{
		resultsWindow.SetActive(true);
		resultsWindow.GetComponent<FinalResultsManager>().kills = score;
		resultsWindow.GetComponent<FinalResultsManager>().deaths = deaths;
		resultsWindow.GetComponent<FinalResultsManager>().result = result;
		resultsWindow.GetComponent<FinalResultsManager>().SetResults();
		finito = true;
	}

	[ClientRpc]
	public void RpcFinalResult(bool result)
	{
		SetResults(result);
	}
}
