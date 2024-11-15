//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerSpawnManager : MonoSingleton<PlayerSpawnManager>
//{
//    public GameObject playerPrefab;
    
//    // Start is called before the first frame update
//    void Start()
//    {
//        if (GameManager.Instance != null && GameManager.Instance.isComingFromPipe)
//        {
//            int targetPipeID = GameManager.Instance.enteredPipeID;
//            Pipe targetPipe = GameUtils.FindPipeByID(targetPipeID);
//            if (targetPipe != null)
//            {
//                Vector3 spawnPosition = targetPipe.transform.position + Vector3.right * 1.5f;
//                Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

//                GameManager.Instance.isComingFromPipe = false;

//            }
//            else
//            {
//                Debug.LogError($"ID가 {targetPipeID}인 파이프를 찾을 수 없습니다.");
//            }
//        }
//        gameObject.SetActive( false );
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}
