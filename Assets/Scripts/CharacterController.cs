using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {
    public float speed = 10f;
    private float gravity = 9.8f;
    public float maxVelocityChange = 10f;
    private bool canJump = false;
    public float jumpForce = 20f;
    private bool onGround = false;

    private Rigidbody rb;

    private float angle = 0f;
    private GameObject light;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        light = GameObject.Find("Light");
    }

    void Awake() {
        GetComponent<Rigidbody>().freezeRotation = true;
    }

    void FixedUpdate() {
        // .75f keeps the player on the slopes
        var targetVelocity = new Vector3(speed * Input.GetAxis("Horizontal"), rb.velocity.y - .75f, speed * Input.GetAxis("Vertical"));
        rb.velocity = targetVelocity;

        if (onGround) {
            if (Input.GetKey(KeyCode.Space)) {
                rb.AddForce(0, jumpForce, 0);
                onGround = false;
            }
        }

        if (transform.position.y < -10) {
            transform.position = new Vector3(0, 5, 0);
            rb.velocity = Vector3.zero;
        }

        // angle += 4f * Input.GetAxis("Mouse X");
        // light.transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    void OnCollisionStay() {
        onGround = true;
    }

    // Update is called once per frame
    void Update() {

    }
}