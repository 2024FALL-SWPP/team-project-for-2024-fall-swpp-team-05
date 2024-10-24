using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformModule : MonoBehaviour
{
    public float g_scale = 1.0f;
    public float deceleration = 1.0f;
    public float maxSpeedX = 10.0f;
    public float maxSpeedY = 10.0f;
    public bool jumpAllowed = false;

    private Rigidbody rb;
    private float gravity = 9.81f;
    [SerializeField] private float speedX = 0.0f;
    [SerializeField] private float speedY = 0.0f;
    private float accelSumX = 0.0f;
    private float accelSumY = 0.0f;

    private float deltaTime = 0f;
    private bool deaccelerating = false;
    private Quaternion targetRotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Apply acceleration
        speedX += accelSumX;
        speedY += accelSumY;

        // Apply deceleration
        if(accelSumX == 0 && deaccelerating)
        {
            deltaTime += Time.deltaTime;
            speedX = Mathf.Lerp(speedX, 0, deltaTime * deceleration);
        }
        else if(accelSumX == 0)
        {
            deaccelerating = true;
        }
        else
        {
            deaccelerating = false;
            deltaTime = 0;
        }

        // Rotate the player
        if(accelSumX > 0)
        {
            rb.MoveRotation(Quaternion.Euler(0, 0, 0));
        }
        else if(accelSumX < 0)
        {
            rb.MoveRotation(Quaternion.Euler(0, 180, 0));
        }

        // Apply gravity when jumping
        speedY -= gravity * g_scale;

        // Apply speed limits
        if(speedX > maxSpeedX)
        {
            speedX = maxSpeedX;
        }
        else if(speedX < -maxSpeedX)
        {
            speedX = -maxSpeedX;
        }
        if(speedY > maxSpeedY)
        {
            speedY = maxSpeedY;
        }
        else if(speedY < -maxSpeedY)
        {
            speedY = -maxSpeedY;
        }

        accelSumX = 0;
        accelSumY = 0;
        rb.velocity = new Vector3(speedX, speedY, 0);
    }

    public void Accelerate(float Xinput, float YInput)
    {
        accelSumX += Xinput;
        accelSumY += YInput;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // check layer collision with ground
        if(!jumpAllowed && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            jumpAllowed = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(jumpAllowed && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            jumpAllowed = false;
        }
    }
}
