using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InGameBehaviour : MonoBehaviour
{
    public GameObject changePassWindow;
    public GameObject[] mainButtons;

    private bool shown = false;

    void Start()
    {
        changePassWindow.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if (shown) {
                onClickResumeButton();
            } else {
                shown = !shown;
            }
        }

        if (shown) {
            changePassWindow.SetActive(true);
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void onClickResumeButton()
    {
        shown = false;
        changePassWindow.SetActive(false);
    }

    public void onClickOptionsButton()
    {
        foreach (GameObject button in mainButtons)
        {
            button.SetActive(false);
        }
    }

    public void onClickSaveCancelButtons()
    {
        foreach (GameObject button in mainButtons)
        {
            button.SetActive(true);
        }
    }

    public void onClickLeaveButton()
    {
        GameObject player = NetworkManager.singleton.client.connection.playerControllers[0].gameObject;
        if (player.GetComponent<walk>().isServer) { return; }
        NetworkManager.singleton.StopClient();
    }
}
