using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameObject stageInputFieldObj, indexInputFieldObj, saveButton, loadButton;

    public void InstantiateTerrain()
    {
        stageInputFieldObj.SetActive(false);
        indexInputFieldObj.SetActive(false);
        loadButton.SetActive(false);
    }
}
