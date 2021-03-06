using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsWindow : MonoBehaviour
{
    public GameObject optionsWindow;
    
    public string configurationFile;
    public string config;
    public Configuration conf = new Configuration();
    public GameObject[] inputs;
    public GameObject sensitivitySlider;
    public GameObject volumeSlider;
    public float sensitivityValue;
    public float volumeValue;

    void Start()
    {
        configurationFile = Application.persistentDataPath + "/config.json";
        InitializeConfiguration();
    }

    // Update is called once per frame
    void Update()
    {
        sensitivityValue = sensitivitySlider.GetComponent<Slider>().value;
        volumeValue = volumeSlider.GetComponent<Slider>().value;
    }

    public void onValueChanged(GameObject inputChanged)
    {
        {
            string inputText = inputChanged.GetComponent<InputField>().text;
            if (inputChanged.GetComponent<InputField>().text.Length > 1)
            {
                inputChanged.GetComponent<CapitalLetter>().current = inputText.Substring(0, 1);
                inputChanged.GetComponent<InputField>().text = inputText.Substring(inputText.Length - 1, 1);
            }
        }
        foreach(GameObject input in inputs)
        {
            if (input != inputChanged)
            {
                if (input.GetComponent<InputField>().text.ToUpper().Equals(inputChanged.GetComponent<InputField>().text.ToUpper()))
                {
                    input.GetComponent<InputField>().text = inputChanged.GetComponent<CapitalLetter>().current.ToUpper();
                    input.GetComponent<CapitalLetter>().current = inputChanged.GetComponent<CapitalLetter>().current.ToUpper();
                }
            }
        }
        if (inputChanged.GetComponent<InputField>().text.Length != 0)
        {
            inputChanged.GetComponent<CapitalLetter>().current = inputChanged.GetComponent<InputField>().text;
        }
    }

    public void updateValue(GameObject inputChanged)
    {
        if (inputChanged.GetComponent<InputField>().text.Length == 0)
        {
            inputChanged.GetComponent<InputField>().text = inputChanged.GetComponent<CapitalLetter>().current;
        }
        else
        {
            inputChanged.GetComponent<CapitalLetter>().current = inputChanged.GetComponent<InputField>().text;
        }
    }

    public void onClickSaveButton()
    {
        Configuration newConfiguration = new Configuration(
            inputs[0].GetComponent<InputField>().text.ToUpper(),
            inputs[1].GetComponent<InputField>().text.ToUpper(),
            inputs[2].GetComponent<InputField>().text.ToUpper(),
            inputs[3].GetComponent<InputField>().text.ToUpper(),
            sensitivityValue, volumeValue);
        config = JsonUtility.ToJson(newConfiguration);
        StreamWriter streamWriter = new StreamWriter(configurationFile);
        streamWriter.WriteLine(config);
        streamWriter.Close();
        InitializeConfiguration();
        optionsWindow.SetActive(false);
    }

    public void onClickCancelButton()
    {
        sensitivitySlider.GetComponent<Slider>().value = conf.sensitivity;
        volumeSlider.GetComponent<Slider>().value = conf.volume;
        optionsWindow.SetActive(false);
    }

    public void onClickSettingsButton()
    {
        optionsWindow.SetActive(true);
        InitializeConfiguration();
    }

    void InitializeConfiguration()
    {
        StreamReader streamReader = new StreamReader(configurationFile);
        config = streamReader.ReadLine();
        JsonUtility.FromJsonOverwrite(config, conf);
        InitializeSettings();
        streamReader.Close();
    }

    void InitializeSettings()
    {
        inputs[0].GetComponent<InputField>().text = conf.forwards.ToUpper();
        inputs[0].GetComponent<CapitalLetter>().current = conf.forwards.ToUpper();
        inputs[1].GetComponent<InputField>().text = conf.backwards.ToUpper();
        inputs[1].GetComponent<CapitalLetter>().current = conf.backwards.ToUpper();
        inputs[2].GetComponent<InputField>().text = conf.left.ToUpper();
        inputs[2].GetComponent<CapitalLetter>().current = conf.left.ToUpper();
        inputs[3].GetComponent<InputField>().text = conf.right.ToUpper();
        inputs[3].GetComponent<CapitalLetter>().current = conf.right.ToUpper();

        sensitivitySlider.GetComponent<Slider>().value = conf.sensitivity;
        volumeSlider.GetComponent<Slider>().value = conf.volume;
    }
}
