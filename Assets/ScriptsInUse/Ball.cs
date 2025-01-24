
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] Paddle paddle1;
    [SerializeField] float xPush = 2f;
    [SerializeField] float yPush = 15f;
    [SerializeField] GameObject BallPosition;
    [SerializeField] float maxBallSpeed = 20f;
    [SerializeField] float randomFactor = 1f;

    private Rigidbody2D rb;
    private bool hasStarted = false;
    private Vector2 previousPaddlePosition;
    //cached component references
    Rigidbody2D myRigidBody2D;
    AudioSource myAudioSource;
    void Start()
    {
        Vector2 paddleToBallVector = transform.position - paddle1.transform.position;
        myRigidBody2D = GetComponent<Rigidbody2D>();
        myAudioSource = GetComponent<AudioSource>();
        myRigidBody2D.velocity = new Vector2(xPush, yPush);
        hasStarted = true;
    }

    void Update()
    {
        if (!hasStarted)
        {
            LockBallpaddle();
            LaunchOnSpaceClick();
        }
    }

    private void LaunchOnSpaceClick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            myRigidBody2D.velocity = new Vector2(xPush, yPush);
            hasStarted = true;
        }
    }

    private void LockBallpaddle()
    {
        transform.position = BallPosition.transform.position;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 velocityTweak = new Vector2(
            UnityEngine.Random.Range(0f, randomFactor),
            UnityEngine.Random.Range(0f, randomFactor)
        );

        if (hasStarted)
        {
            // Play collision sound
            myAudioSource.Play();

            // Add a slight tweak to avoid predictable bouncing
            myRigidBody2D.velocity += velocityTweak;

            // Enforce minimum speed to prevent cycling behavior
            EnforceMinimumVelocity();
        }
    }

    private void EnforceMinimumVelocity()
    {
        // Ensure the ball's speed doesn't drop too low in any direction
        float minVelocity = 2f; // Adjust as needed
        Vector2 currentVelocity = myRigidBody2D.velocity;

        // Avoid extremely low or cycling velocities
        if (Mathf.Abs(currentVelocity.x) < minVelocity)
        {
            float correction = currentVelocity.x > 0 ? minVelocity : -minVelocity;
            currentVelocity.x = correction;
        }
        if (Mathf.Abs(currentVelocity.y) < minVelocity)
        {
            float correction = currentVelocity.y > 0 ? minVelocity : -minVelocity;
            currentVelocity.y = correction;
        }

        // Apply the corrected velocity
        myRigidBody2D.velocity = currentVelocity;

        // Limit maximum speed if needed
        if (currentVelocity.magnitude > maxBallSpeed)
        {
            myRigidBody2D.velocity = currentVelocity.normalized * maxBallSpeed;
        }
    }


}

