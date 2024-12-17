using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBlockManager : MonoBehaviour
{
    public float fallSpeed = 10.0f;               // 낙하 속도
    public float fallDelay = 0.5f;               // 낙하 시작 대기 시간
    public float resetTimeOutsideView = 3.0f;    // 화면 밖에서 대기 후 초기화 시간
    public float triggerHeightOffset = 0.5f;     // 트리거 오프셋

    private Vector3 startPosition;               // 초기 위치 저장
    private bool isFalling = false;              // 현재 낙하 중인지 여부
    private float timeOutsideView = 0.0f;        // 화면 밖에 머문 시간
    private bool playerOnTop = false;            // 플레이어가 블록 위에 있는지 여부

    private Collider blockPhysicalCollider, blockTriggerCollider; // 블록의 3D Collider
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        // Rigidbody를 가져와 Kinematic으로 설정합니다.
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // 처음 위치 저장
        startPosition = transform.position;

        // 모든 Collider를 가져옵니다.
        Collider[] colliders = GetComponents<Collider>();
        blockPhysicalCollider = colliders[0];
        blockTriggerCollider = colliders[1];

        blockTriggerCollider.enabled = true;
        blockPhysicalCollider.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFalling)
        {
            // 블록 낙하
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            // let PC be sticked to the block
            if (playerOnTop && player != null)
            {
                player.GetComponent<Rigidbody>().MovePosition(player.transform.position + Vector3.down * fallSpeed * Time.deltaTime);
            }

            // 화면 밖에서 일정 시간 동안 머물면 초기 위치로 복귀
            if (!IsVisibleToCamera())
            {
                timeOutsideView += Time.deltaTime;
                if (timeOutsideView >= resetTimeOutsideView)
                {
                    ResetBlock();
                }
            }
            else
            {
                timeOutsideView = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 블록 위에 있는 경우
        if (other.GetComponent<Collider>().CompareTag("Player"))
        {
            // 플레이어의 Y 좌표가 블록의 상단 Y 좌표보다 높은 경우에만 playerOnTop 설정
            Debug.Log("Player Enter");
            if (other.transform.position.y + triggerHeightOffset > transform.position.y) {
                playerOnTop = true;
                player = other.gameObject;
                Debug.Log("Player Stay");
                blockPhysicalCollider.enabled = true;  // 충돌 활성화
                Invoke("StartFalling", fallDelay);  // 일정 시간 후 낙하 시작
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 플레이어가 블록에서 내려가면 낙하 취소
        if (other.GetComponent<Collider>().CompareTag("Player"))
        {
            playerOnTop = false;
            player = null;
            blockTriggerCollider.enabled = true; // activate trigger
            Debug.Log("Player Exit");
            CancelInvoke("StartFalling");
        }
    }

    void StartFalling()
    {
        if (playerOnTop && !isFalling)
        {
            isFalling = true;
            // blockPhysicalCollider.enabled = false;  // 낙하 중 충돌 비활성화
        }
    }

    private bool IsVisibleToCamera()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.y > 0;
    }

    void ResetBlock()
    {
        // 초기 위치로 되돌림
        transform.position = startPosition;
        isFalling = false;
        playerOnTop = false;
        timeOutsideView = 0f;
        blockPhysicalCollider.enabled = false;
        blockTriggerCollider.enabled = true; // activate trigger
    }
}
