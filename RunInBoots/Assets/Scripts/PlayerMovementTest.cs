using UnityEngine;

public class PlayerMovementTest : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // x�� �������� �̵��ϴ� �׽�Ʈ �ڵ� (�¿� Ű ���)
        float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(move, 0, 0);
    }
}