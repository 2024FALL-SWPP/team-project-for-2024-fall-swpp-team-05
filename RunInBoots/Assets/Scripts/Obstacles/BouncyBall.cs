using UnityEngine;

public class BouncyBall : MonoBehaviour
{
    public int ballActionKey = 1023;          // 고무공 액션 키
    public float repelForce = 150f;         // 밀어내는 힘의 크기    
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            RepelPlayer(collision);
            ExecuteActionOnBall(ballActionKey);
        }
    }

    private void RepelPlayer(Collision collision)
    {
        // 충돌 방향 계산
        Vector3 repelDirection = (collision.transform.position - transform.position).normalized;
        
        
        // choose left/right/up/down direction
        if (Mathf.Abs(repelDirection.x) > Mathf.Abs(repelDirection.y))
        {
            repelDirection.y = 0;
            repelDirection.x = repelDirection.x > 0 ? 1 : -1;
        }
        else
        {
            repelDirection.x = 0;
            repelDirection.y = repelDirection.y > 0 ? 1 : -1;
        }
        Debug.Log("Repel Direction: " + repelDirection);

        // PC의 Rigidbody에 힘을 가하여 밀어냄
        Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            Debug.Log("Repel Player");
            playerRb.AddForce(repelDirection * repelForce, ForceMode.Impulse);
        }
    }

    private void ExecuteActionOnBall(int actionKey)
    {
        // 고무공에 대한 특정 액션을 실행
        Debug.Log("고무공에 대한 액션 실행: " + actionKey);
        // GetComponent<ActionSystem>().SetAction(actionKey);
    }
}
