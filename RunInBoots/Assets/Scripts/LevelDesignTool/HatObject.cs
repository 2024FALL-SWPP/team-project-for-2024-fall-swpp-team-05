using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatObject : MonoBehaviour
{
    public eHatType hatType;              // 이 오브젝트가 표현하는 모자 유형

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어와 충돌했는지 확인
        CamouflageModule camouflageModule = other.GetComponent<CamouflageModule>();
        if (other.gameObject.CompareTag("Player") && camouflageModule != null)
        {
            // 모자 장착 시도
            camouflageModule.EquipHat(hatType);

            // 모자 오브젝트 비활성화
            gameObject.SetActive(false);
        }
    }
}