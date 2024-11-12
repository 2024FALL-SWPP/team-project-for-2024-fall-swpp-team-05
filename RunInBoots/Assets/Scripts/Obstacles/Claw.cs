using UnityEngine;

public class Claw : MonoBehaviour
{
    public int pcActionKey = 103;          // PC에게 실행시킬 액션 키
    public int clawActionKey = 102;        // Claw에게 실행시킬 액션 키

    private GameObject player;           // PC 참조
    private Transform parent;            // PC의 부모 참조
    public bool isGrabbing = false;     // 현재 붙잡고 있는 상태인지
    private ConfigurableJoint configurableJoint;       // PC를 흔들리게 할 Configurable Joint

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isGrabbing)
        {
            GrabPlayer(other.gameObject);
        }
    }

    private void Update()
    {
        if (isGrabbing)
        {
            // free PC
            if (!IsPlayerInAction() || !IsClawInAction())
            {
                ReleasePlayer();
            }
        }
    }

    private void GrabPlayer(GameObject pc)
    {
        player = pc;
        isGrabbing = true;

        // 플레이어를 Claw의 자식으로 설정
        parent = player.transform.parent;
        player.transform.SetParent(transform);
        player.transform.localPosition = Vector3.zero;

        configurableJoint = player.gameObject.AddComponent<ConfigurableJoint>();
        configurableJoint.connectedBody = this.GetComponent<Rigidbody>();

        // 세부 설정
        configurableJoint.axis = Vector3.right;
        configurableJoint.secondaryAxis = Vector3.up;

        configurableJoint.xMotion = ConfigurableJointMotion.Locked;
        configurableJoint.yMotion = ConfigurableJointMotion.Locked;
        configurableJoint.zMotion = ConfigurableJointMotion.Locked;

        configurableJoint.angularXMotion = ConfigurableJointMotion.Free;
        configurableJoint.angularYMotion = ConfigurableJointMotion.Free;
        configurableJoint.angularZMotion = ConfigurableJointMotion.Free;

        // PC와 Claw에 각각 지정된 액션 실행
        ExecuteActionOnPC(pcActionKey);
        ExecuteActionOnClaw(clawActionKey);
    }

    private void ReleasePlayer()
    {
        if (player != null)
        {
            // 부모-자식 관계 해제
            Debug.Log("Release Player");
            player.transform.SetParent(parent);

            // Hinge Joint 제거
            if (configurableJoint != null)
            {
                Destroy(configurableJoint);
            }

            isGrabbing = false;
        }
    }

    private void ExecuteActionOnPC(int actionKey)
    {
        // PC에 대한 특정 액션을 실행 (추후 PC에 특정 액션 메서드를 추가해야 함)
        Debug.Log("PC에 대한 액션 실행: " + actionKey);
        player.GetComponent<ActionSystem>().SetAction(actionKey);
    }

    private void ExecuteActionOnClaw(int actionKey)
    {
        // Claw에 대한 특정 액션을 실행
        Debug.Log("Claw에 대한 액션 실행: " + actionKey);
        GetComponent<ActionSystem>().SetAction(actionKey);
    }

    private bool IsPlayerInAction()
    {
        // PC가 특정 액션 상태인지 확인하는 로직 필요
        if (player != null)
        {   
            Debug.Log("Player Action Key: " + player.GetComponent<ActionSystem>().currentAction.Key);
            return player.GetComponent<ActionSystem>().currentAction.Key == pcActionKey;
        }
        return false;
    }

    private bool IsClawInAction()
    {
        // Claw가 특정 액션 상태인지 확인하는 로직 필요
        // return GetComponent<ActionSystem>().currentAction.Key == clawActionKey;
        return true;
    }
}