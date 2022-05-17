using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Player : MonoBehaviour {

    public static Vector3 playerSize;
    private static Vector2 startSwipePos = Vector2.zero;

    private static float rotationSpeed = 35;
    private static float maxRotation = 50;

    public static bool isRunning = false;

    public float acceleration;
    public float topSpeed;

    public void Start() {
        topSpeed = Controller.GetScreenSize().x * 3.5f;
        acceleration = topSpeed * 3f;
    }

    public void Update() {
        if (!Controller.isGameOver) {
            ProcessInput();
        }
    }

    public void FixedUpdate() {
        if (!Controller.isGameOver) {
            move();
        }
    }

    private void ProcessInput() {

        Animator animator = this.GetComponent<Animator>();

        (bool, string) swipe = Controller.GetSwipe();
        if (swipe.Item1) {
            if (isRunning) {
                if (swipe.Item2.Equals("SWIPE_DOWN")) {
                    animator.SetTrigger("slide");
                } else if (swipe.Item2.Equals("SWIPE_UP")) {
                    animator.SetTrigger("jump");
                }
            }
        } else if(swipe.Item2.Equals("TAP")) {
            animator.SetBool("isRunning", !animator.GetBool("isRunning"));
            isRunning = !isRunning;
        } else if(swipe.Item2.Equals("HOLD")) {

            float rotationDelta = rotationSpeed * Time.deltaTime;
            float currentRotation = transform.rotation.eulerAngles.y;

            if(Input.mousePosition.x < Screen.width / 2) {
                rotationDelta *= -1;
            }

            bool canRotate = rotationDelta >= 0
                ? (currentRotation < maxRotation || currentRotation > 180)
                : (currentRotation > 180 + maxRotation || currentRotation < 180);

            if(canRotate) {
                transform.Rotate(Vector3.up, rotationDelta);
            }

        }

    }

    private void move() {

        if(isRunning) {

            Vector3 velocity = this.GetComponent<Rigidbody>().velocity;
            Vector3 targetSpeed = GetComponent<Animator>().GetBool("isRunning") ? GetRadiusVector(this.topSpeed, true) : Vector3.zero;
            Vector3 acceleration = GetRadiusVector(this.acceleration, false);

            Vector3 speedDelta = targetSpeed - velocity;

            if(Mathf.Abs(speedDelta.x) > acceleration.x * Time.deltaTime) {
                if(speedDelta.x > 0) {
                    velocity.x += acceleration.x * Time.deltaTime;
                } else {
                    velocity.x -= acceleration.x * Time.deltaTime;
                }
            }

            if(Mathf.Abs(speedDelta.z) > acceleration.y * Time.deltaTime) {
                if(speedDelta.z > 0) {
                    velocity.z += acceleration.z * Time.deltaTime;
                } else {
                    velocity.z -= acceleration.z * Time.deltaTime;
                }
            }
        
            this.GetComponent<Rigidbody>().velocity = velocity;

        } else {
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    private Vector3 GetRadiusVector(float radius, bool relativeToPlayer) {

        float angle = 45;

        if (relativeToPlayer) {
            angle = transform.rotation.eulerAngles.y;
            angle = 360 - (angle - 90);
            if (angle > 360) {
                angle -= 360;
            }
        } 

        float x = radius * Mathf.Cos(Mathf.Deg2Rad * angle);
        float z = radius * Mathf.Sin(Mathf.Deg2Rad * angle);

        return new Vector3(x, 0, z);
    }

}