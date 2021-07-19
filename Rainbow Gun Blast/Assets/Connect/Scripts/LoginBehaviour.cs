using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginBehaviour : MonoBehaviour
{

	public InputField usernameField;
	public InputField passwordField;
    public GameObject loadingSplash;
    public GameObject playerAccount;
    private string IP = "95.76.245.97";

    public GameObject showErrs;
    public void CallLogIn() {
        StartCoroutine(Login());
    }

    IEnumerator Login() {
        loadingSplash.GetComponent<LoadingSplash>().isLoading = true;

        WWWForm form = new WWWForm();

        form.AddField("username", usernameField.text);
        form.AddField("password", passwordField.text);

        var www = UnityWebRequest.Post("http://" + IP + ":8888/rgb/login.php", form);
        
        yield return www.SendWebRequest();

        if (www.downloadHandler.text == "0") {
            showErrs.GetComponent<Text>().text="User logged in succesfully";
            playerAccount.GetComponent<PlayerData>().username = usernameField.text;
            loadingSplash.GetComponent<LoadingSplash>().isLoading = false;
            SceneManager.LoadScene(1);
        } else {
            showErrs.GetComponent<Text>().text="User could not log in. Error: " + www.downloadHandler.text;
            loadingSplash.GetComponent<LoadingSplash>().isLoading = false;
        }
    }
}
