using TMPro;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    //public void Start()
    //{
    //    //Initialize();
    //}

    public void Initialize()
    {
        gameObject.SetActive(false);
        GameManager.Instance.GetCurrentStageState().SpawnPlayer(transform.position);
    }
}
