using UnityEngine;

public class StretchModule : MonoBehaviour
{
    // Public variables for configuration
    public float maxStretchLength = 5f; // 최대 길이 (m)
    public float returnSpeed = 0.1f; // 돌아가기 속도 (Lerp 계수)
    public Transform targetBone; // 상체 본 (이동시키기 위한 본)

    // Private variables to track stretching state
    public float currentStretchAmount = 0f; // 현재 늘어난 길이
    private Vector3 initialColliderSize; // 콜라이더 초기 크기
    private ActionSystem actionSystem; // 액션 시스템
    private Vector3 initialBonePosition; // 본 초기 위치
    private BoxCollider unitCollider; // 유닛의 캡슐 콜라이더

    public Rigidbody rb;

    public bool isStretching = false;

    void Start()
    {
        // Initialize collider and bone setting
        unitCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        actionSystem = GetComponent<ActionSystem>();
        if (unitCollider != null && actionSystem != null)
        {
            initialColliderSize = new Vector3 (actionSystem.currentAction.ColliderX, actionSystem.currentAction.ColliderY, 1); // 캡슐 콜라이더의 초기 크기
        }
        else if(actionSystem == null)
        {
            initialColliderSize = unitCollider.size;
        }
        if (targetBone != null)
        {
            initialBonePosition = targetBone.localPosition; // 상체 본의 초기 위치
        }
    }

    // **늘리기** 기능: 늘릴 길이만큼 스트레칭 적용
    public void Stretch(float stretchAmount)
    {
        // 천장 제한에 맞춰야 할 경우
        RaycastHit hit;
        if (Physics.Raycast(transform.position+Vector3.up*(unitCollider.size.y-0.1f), Vector3.up, out hit, stretchAmount+0.1f))
            currentStretchAmount += hit.distance - 0.1f;
        // 늘리기 길이를 누적하고 최대 길이로 제한
        else
            currentStretchAmount +=stretchAmount;

        // 최대 길이 제한
        currentStretchAmount = Mathf.Clamp(currentStretchAmount, 0, maxStretchLength);

        isStretching = true;
    }

    // **상태 갱신** 기능: 매 프레임마다 위치와 크기 갱신
    void LateUpdate()
    {

        // 유닛의 콜라이더 크기 및 본 위치 조정
        if (unitCollider != null)
        {
            if(actionSystem != null)
            {
                initialColliderSize = new Vector3(actionSystem.currentAction.ColliderX, actionSystem.currentAction.ColliderY, 1);
            }
            unitCollider.size = new Vector3(unitCollider.size.x, initialColliderSize.y + currentStretchAmount, unitCollider.size.z);
            unitCollider.center = new Vector3(0, unitCollider.size.y / 2, 0);
        }
        if (targetBone != null)
        {
            targetBone.localPosition = initialBonePosition + Vector3.up * currentStretchAmount / targetBone.lossyScale.y;
        }

    }

    void FixedUpdate() 
    {
        if (currentStretchAmount > 0 && !isStretching)
        {
            float old = currentStretchAmount;
            currentStretchAmount = Mathf.Lerp(currentStretchAmount, 0, returnSpeed * Time.deltaTime);
            old = old - currentStretchAmount;
            // 유닛 Y축 위치 조정
            rb.MovePosition(rb.position + Vector3.up * old);
        }

        isStretching = false;
    }

    // 예시: 특정 키로 늘리기 동작 테스트
    void Update()
    {
        isStretching=false;
        if (Input.GetKey(KeyCode.Space)) // 스페이스 키를 눌렀을 때
        {
            Stretch(0.1f); // 1.0f만큼 늘리기
        }
    }
}