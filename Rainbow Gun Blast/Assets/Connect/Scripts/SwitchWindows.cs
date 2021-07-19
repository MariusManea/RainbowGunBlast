using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWindows : MonoBehaviour
{
    public GameObject LoginWindow;
    public GameObject RegisterWindow;
    public Vector3 screenPosition;
    public Vector3 loginOffScreenPosition;
    public Vector3 signupOffScreenPosition;
    public bool switched;
    public float speed;
    public GameObject loadSplah;
    private bool isLoading;
    void Start()
    {
        switched = true;
    }

    void Update()
    {
        isLoading = loadSplah.GetComponent<LoadingSplash>().isLoading;
    }

    IEnumerator LoginSwitchCoroutine()
    {
        while (LoginWindow.transform.localPosition.x - screenPosition.x < 0)
        {
            if (!switched)
            {
                LoginWindow.transform.Translate(Vector3.right * speed * Time.deltaTime);
                RegisterWindow.transform.Translate(Vector3.right * speed * Time.deltaTime);
            }
            yield return true;
        }
        switched = true;
        yield return true;
    }

    public void SwitchToLogin()
    {
        if (switched && !isLoading)
        {
            switched = false;
            StartCoroutine(LoginSwitchCoroutine());
        }
    }

    IEnumerator SignupSwitchRoutine()
    {
        while (RegisterWindow.transform.localPosition.x - screenPosition.x > 0)
        {
            if (!switched)
            {
                LoginWindow.transform.Translate(Vector3.left * speed * Time.deltaTime);
                RegisterWindow.transform.Translate(Vector3.left * speed * Time.deltaTime);
            }
            yield return true;
        }
        switched = true;
        yield return true;
    }

    public void SwitchToSignup()
    {
        if (switched && !isLoading)
        {
            switched = false;
            StartCoroutine(SignupSwitchRoutine());
        }
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
