using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlockManager : MonoBehaviour
{
    public float moveSpeed = 2.0f;           // 이동 속도
    public Vector2 moveAmount = new Vector2(3, 0);  // 이동할 거리
    public float waitTime = 1.0f;            // 이동 사이의 대기 시간

    private Vector2 startPosition;            // 시작 위치
    private Vector2 targetPosition;           // 목표 위치

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;                // 처음 위치 저장
        targetPosition = startPosition + moveAmount;       // 목표 위치 계산
        StartCoroutine(MoveBlock());
        
    }


    IEnumerator MoveBlock()
    {
        while (true)
        {
            // 시작 위치에서 목표 위치로 이동
            yield return StartCoroutine(MoveToPosition(targetPosition));

            // 대기
            yield return new WaitForSeconds(waitTime);

            // 목표 위치에서 시작 위치로 이동
            yield return StartCoroutine(MoveToPosition(startPosition));

            // 대기
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator MoveToPosition(Vector2 destination)
    {
        while ((Vector2)transform.position != destination)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null; // 다음 프레임까지 대기
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
