using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBlockManager : MonoBehaviour
{
    public float fallSpeed = 5.0f;               // 낙하 속도
    public float fallDelay = 2.0f;               // 낙하 시작 대기 시간
    public float resetTimeOutsideView = 3.0f;    // 화면 밖에서 대기 후 초기화 시간

    private Vector3 startPosition;               // 초기 위치 저장
    private bool isFalling = false;              // 현재 낙하 중인지 여부
    private float timeOutsideView = 0.0f;        // 화면 밖에 머문 시간
    private bool playerOnTop = false;            // 플레이어가 블록 위에 있는지 여부
    private Rigidbody playerRigidbody;           // 플레이어의 Rigidbody 참조

    private Collider blockCollider;              // 블록의 3D Collider

    // Start is called before the first frame update
    void Start()
    {
        // Rigidbody를 가져와 Kinematic으로 설정합니다.
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // 처음 위치 저장
        startPosition = transform.position;

        // Collider 가져오기
        blockCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFalling && playerRigidbody != null)
        {
            // 플레이어와 같은 속도로 낙하
            Vector3 fallVelocity = new Vector3(0, playerRigidbody.velocity.y, 0);
            transform.position += fallVelocity * Time.deltaTime;

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

    private void OnCollisionStay(Collision other)
    {
        // 플레이어가 블록 위에 있는 경우
        if (other.collider.CompareTag("Player"))
        {
            playerOnTop = true;
            playerRigidbody = other.collider.GetComponent<Rigidbody>(); // 플레이어의 Rigidbody 참조 저장
            Invoke("StartFalling", fallDelay);  // 일정 시간 후 낙하 시작
        }
    }

    private void OnCollisionExit(Collision other)
    {
        // 플레이어가 블록에서 내려가면 낙하 취소
        if (other.collider.CompareTag("Player"))
        {
            playerOnTop = false;
            CancelInvoke("StartFalling");
        }
    }

    void StartFalling()
    {
        if (playerOnTop && !isFalling)
        {
            isFalling = true;
            blockCollider.enabled = false;  // 낙하 중 충돌 비활성화
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
        timeOutsideView = 0f;
        blockCollider.enabled = true;  // 충돌 다시 활성화
        playerRigidbody = null;        // 플레이어 Rigidbody 참조 초기화
    }
}
