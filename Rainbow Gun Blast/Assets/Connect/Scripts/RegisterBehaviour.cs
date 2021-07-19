using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RegisterBehaviour : MonoBehaviour
{
    public InputField usernameField;
    public InputField emailField;
    public InputField passwordField;
    public InputField confirmPasswordField;
    public GameObject loadingSplash;
    public GameObject showErrs;

    private string IP = "95.76.245.97";
    public void CallRegister() {
        if (Check() < 0) {
            return;
        }

        StartCoroutine(Register());
    }

    IEnumerator Register() {
        loadingSplash.GetComponent<LoadingSplash>().isLoading = true;

        WWWForm form = new WWWForm();

        form.AddField("username", usernameField.text);
        form.AddField("email", emailField.text);
        form.AddField("password", passwordField.text);

        var www = UnityWebRequest.Post("http://" + IP + ":8888/rgb/register.php", form);

        yield return www.SendWebRequest();

        if (www.downloadHandler.text == "0") {
            showErrs.GetComponent<Text>().text="User created succesfully";
            loadingSplash.GetComponent<LoadingSplash>().isLoading = false;
        } else {
            showErrs.GetComponent<Text>().text="User not created. Error: " + www.downloadHandler.text;
            loadingSplash.GetComponent<LoadingSplash>().isLoading = false;
        }
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                  RegexOptions.None);

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                var domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase);
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
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

    int Check() {

        // username between 8 and 20 char
        if (usernameField.text.Length < 8 || usernameField.text.Length > 20) {
            showErrs.GetComponent<Text>().text="Please insert a username with more than 8 chars";
            return -1;
        }

        if (!Equals(passwordField.text, confirmPasswordField.text)) {
            showErrs.GetComponent<Text>().text="The passwords don't match. Please retry.";
            return -1;
        }

        if (!IsValidEmail(emailField.text)) {
            showErrs.GetComponent<Text>().text="Please insert a valid e-mail.";
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
