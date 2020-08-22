using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {
    public Transform target;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    public Vector3 offset = new Vector3(-30, 30, -30);
    // Start is called before the first frame update
    void Start() {

    }

    void Update() {
        // Define a target position above and behind the target transform
        Vector3 targetPosition = target.TransformPoint(offset);

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}