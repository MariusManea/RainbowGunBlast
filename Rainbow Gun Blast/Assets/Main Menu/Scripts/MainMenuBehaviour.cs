using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MainMenuBehaviour : MonoBehaviour
{
    public GameObject[] mainButtons;
    public GameObject[] playButtons;
    public GameObject[] userButtons;

    public GameObject gameSettings;
    public GameObject usernameField;

    public GameObject showErrs;

    private string IP = "95.76.245.97";

    void Start()
    {
        foreach (GameObject button in userButtons)
        {
            button.SetActive(false);
        }
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void onClickLogoutButton()
    {
        // trebuie luat camp din PlayerAccount
        //usernameField = GameObject.FindObjectWithTag("")
        /* logout from database logic */
        StartCoroutine(Logout());
    }

    IEnumerator Logout() {
        WWWForm form = new WWWForm();
        usernameField = GameObject.Find("PlayerAccount");
        form.AddField("username", usernameField.GetComponent<PlayerData>().username);

        var www = UnityWebRequest.Post("http://" + IP + ":8888/rgb/logout.php", form);

        yield return www.SendWebRequest();

        if (www.downloadHandler.text == "0") {
            showErrs.GetComponent<Text>().text="User logged out";
            GameObject.Destroy(usernameField);
            SceneManager.LoadScene(0);
        } else {
            showErrs.GetComponent<Text>().text="User could not logout. Error: " + www.downloadHandler.text;            
        }
    }

    public void onClickPlayButton()
    {
        foreach (GameObject button in mainButtons)
        {
            button.SetActive(false);
        }
        foreach (GameObject button in playButtons)
        {
            button.SetActive(true);
        }
    }

    public void onClickCancelPlayButton()
    {
        foreach (GameObject button in playButtons)
        {
            button.SetActive(false);
        }
        foreach (GameObject button in mainButtons)
        {
            button.SetActive(true);
        }
    }

    public void onClickSettingsButton()
    {
        foreach (GameObject button in mainButtons)
        {
            button.SetActive(false);
        }
        foreach (GameObject button in userButtons)
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

    public void onClickChangePassButton()
    {
        foreach (GameObject button in userButtons)
        {
            button.SetActive(false);
        }
    }

    public void onClickChangeCancelButton()
    {
        foreach (GameObject button in userButtons)
        {
            button.SetActive(true);
        }
    }

    public void onClickUserButton()
    {
        foreach (GameObject button in mainButtons)
        {
            button.SetActive(false);
        }
        foreach (GameObject button in userButtons)
        {
            button.SetActive(true);
        }

        var text = userButtons[0].GetComponentsInChildren<Text>();
        // trebuie verificat, afiseaza ce trebuie fara
        usernameField = GameObject.Find("PlayerAccount");
        text[0].text = "Hi, " + usernameField.GetComponent<PlayerData>().username;
    }

    public void onClickUBackButton()
    {
        foreach (GameObject button in mainButtons)
        {
            button.SetActive(true);
        }
        foreach (GameObject button in playButtons)
        {
            button.SetActive(false);
        }
        foreach (GameObject button in userButtons)
        {
            button.SetActive(false);
        }
    }

    public void onClickSoloButton()
    {
        GameObject solo_duo_toggle = playButtons[playButtons.Length - 2];
        solo_duo_toggle.GetComponent<Slider>().value = 0;
    }

    public void onClickTeamsButton()
    {
        GameObject solo_duo_toggle = playButtons[playButtons.Length - 2];
        solo_duo_toggle.GetComponent<Slider>().value = 1;
    }

    public void onClickDeathmatchButton()
    {
        GameObject death_last_toggle = playButtons[playButtons.Length - 1];
        death_last_toggle.GetComponent<Slider>().value = 0;
    }

    public void onClickLastManStandingButton()
    {
        GameObject death_last_toggle = playButtons[playButtons.Length - 1];
        death_last_toggle.GetComponent<Slider>().value = 1;
    }

    public void onSoloTeamValueChange(GameObject type)
    {
        int sign = type.GetComponent<Slider>().value == 1 ? 1 : -1;
        int weight = 10;
        gameSettings.GetComponent<GameSettings>().gameCode += sign * weight;
    }
    public void onDeathLastValueChange(GameObject type)
    {
        int sign = type.GetComponent<Slider>().value == 1 ? 1 : -1;
        int weight = 1;
        gameSettings.GetComponent<GameSettings>().gameCode += sign * weight;
    }
}
