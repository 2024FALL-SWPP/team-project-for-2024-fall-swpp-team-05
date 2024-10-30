using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class StartScreenManager : MonoBehaviour
{
    public Button startButton;
    public TextMeshProUGUI stageInputField, indexInputField;
    private string dataPath = Application.dataPath + "/Resources/Levels";

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartGame()
    {
        string stage = stageInputField.text;
        string index = indexInputField.text;

        string filename = $"Stage_{stage}_{index}.json";

        string filePath = Path.Combine(dataPath, filename);

        Debug.Log($"stage: {stage}, index: {index}, filename: {filename}, filePath: {filePath}");

        // startButton.SetActive(false);
        // stageInputField.SetActive(false);
        // indexInputField.SetActive(false);


        
    }
}
