using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlockManager : MonoBehaviour
{
    public Vector2 moveAmount;          // 블록의 이동량
    public float moveSpeed = 1f;        // 이동 속도
    public float waitTime = 1.0f;       // 이동 후 대기 시간

    private Vector2 startPos;           // 블록의 시작 위치
    private Vector2 targetPos;          // 이동 목표 위치
    private bool movingToTarget = true; // 목표 위치로 이동 중인지 확인
    private bool isWaiting = false;     // 대기 중인지 확인
    private Rigidbody playerRB;          // 플레이어 참조
    private bool playerOnBlock = false; // 플레이어가 블록 위에 있는지 확인
    private Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        // 시작 위치와 목표 위치 설정
        startPos = transform.position;
        targetPos = startPos + moveAmount;

        //rigidbody를 가져옵니다
        rb = GetComponent<Rigidbody>();
    }

    private void MoveBlock()
    {
        // 이동 방향 및 속도 설정
        Vector2 currentPosition = transform.position;
        Vector2 destination = movingToTarget ? targetPos : startPos;
        Vector2 newPosition = Vector3.MoveTowards(currentPosition, destination, moveSpeed * Time.deltaTime);

        // 블록 이동
        Vector3 movementDelta = newPosition - currentPosition;
        rb.MovePosition(newPosition);

        // 블록 위에 플레이어가 있는 경우, 플레이어 위치를 블록 이동에 따라 업데이트
        if (playerOnBlock && playerRB != null)
        {
            Vector3 playerMoveX = new Vector3(movementDelta.x, 0, 0);
            playerRB.position += playerMoveX;
        }

        // 목표 위치에 도달했는지 확인
        if ((Vector2)transform.position == destination)
        {
            movingToTarget = !movingToTarget;
            StartCoroutine(WaitBeforeMoving());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 블록 위에 있는 경우
        if (other.GetComponent<Collider>().CompareTag("Player"))
        {
            // 플레이어의 Y 좌표가 블록의 상단 Y 좌표보다 높은 경우에만 playerOnTop 설정
            if (other.transform.position.y > transform.position.y)
            {
                //Debug.Log("Player on MoveBlock");
                playerRB = other.gameObject.GetComponent<Rigidbody>();
                //blockPhysicalCollider.enabled = true;  // 충돌 활성화
                playerOnBlock = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // 플레이어가 블록에서 내려갔을 때
        if (other.GetComponent<Collider>().CompareTag("Player"))
        {
            //Debug.Log("Player Exit from MoveBlock");
            //blockPhysicalCollider.enabled = false;  // 충돌 비활성화
            playerOnBlock = false;
            playerRB = null;
        }
    }

    private IEnumerator WaitBeforeMoving()
    {
        // 대기 시간 동안 블록 이동을 멈추게 하는 플래그
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isWaiting)
        {
            MoveBlock();
        }
    }
}
