using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CapitalLetter : MonoBehaviour
{
    public string current;

    // Start is called before the first frame update
    void Start()
    {
        current = GetComponent<InputField>().text.ToUpper();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<InputField>().text = GetComponent<InputField>().text.ToUpper();
    }
}
