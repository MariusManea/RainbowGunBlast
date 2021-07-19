using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ChangePasswordBehaviour : MonoBehaviour
{
    public GameObject changePassWindow;
    
    public InputField oldPasswordField;
    public InputField passwordField;
    public InputField confirmPasswordField;
    public GameObject confirmButton;
    public GameObject cancelButton;

    public GameObject showErrs;

    private string IP = "95.76.245.97";

    void Start()
    {
        changePassWindow.SetActive(false);
    }

    public void onClickChangePassButton()
    {
        changePassWindow.SetActive(true);
        //confirmButton.SetActive(true);
        //cancelButton.SetActive(true);
    }

    public void onClickConfirmButton()
    {
        CallRegister();
    }

    public void onClickCancelButton()
    {
        changePassWindow.SetActive(false);
    }

    public int CallRegister() {
        if (Check() < 0) {
            return - 1;
        }

        StartCoroutine(ChangePassword());
        return 0;
    }

    IEnumerator ChangePassword() {
        WWWForm form = new WWWForm();

        GameObject usernameField = GameObject.Find("PlayerAccount");
        form.AddField("username", usernameField.GetComponent<PlayerData>().username);
        form.AddField("oldPassword", oldPasswordField.text);
        form.AddField("newPassword", passwordField.text);

        var www = UnityWebRequest.Post("http://" + IP + ":8888/rgb/changePass.php", form);

        yield return www.SendWebRequest();

        if (www.downloadHandler.text == "0") {
            showErrs.GetComponent<Text>().text="Password changed succesfully";            
            changePassWindow.SetActive(false);

            foreach (GameObject button in GetComponent<MainMenuBehaviour>().mainButtons)
            {
                button.SetActive(true);
            }

        } else {
            showErrs.GetComponent<Text>().text="Password not changed. Error: " + www.downloadHandler.text;
        }
    }

    private static bool isValidPass(string pass) {
        if (string.IsNullOrWhiteSpace(pass))
            return false;

        try
        {
            return Regex.IsMatch(pass, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,30}$",
                RegexOptions.IgnoreCase);
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    int Check()
    {
        // check if passwords match
        if (!Equals(passwordField.text, confirmPasswordField.text)) {
            showErrs.GetComponent<Text>().text="The passwords don't match. Please retry.";
            return -1;
        }

        // pass between 8 and 30 and 1 number, 1 lowercase, 1 uppercase
        if (!isValidPass(passwordField.text)) {
            showErrs.GetComponent<Text>().text="The password should be minimum 8 characters long and should contain one digit, one lower and one upper letter.";
            return -1;
        }

        return 0;
    }
}
