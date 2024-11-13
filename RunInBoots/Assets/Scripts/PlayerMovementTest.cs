using UnityEngine;

public class PlayerMovementTest : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // x축 방향으로 이동하는 테스트 코드 (좌우 키 사용)
        float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(move, 0, 0);
    }
}