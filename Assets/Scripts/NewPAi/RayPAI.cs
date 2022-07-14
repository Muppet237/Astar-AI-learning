using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayPAI:MonoBehaviour {

    public GameObject target;
    public float speed;

    float[] distanceToPlayer = new float[8];
    float maxDistance = 10;

    Rigidbody rgbd;

    void Start() {
        rgbd = GetComponent<Rigidbody>();

    }

    void Update() {
        transform.LookAt(target.transform.position);
        Vector3 forcedDirection = target.transform.position - transform.position;
        rgbd.velocity = forcedDirection * speed;

        int angle = 0;
        for(int i = 0; i < 8; i++) {
            float test = Vector3.Distance(target.transform.position, transform.position);
            distanceToPlayer[i] = Mathf.Clamp(Vector3.Distance(transform.position + Quaternion.Euler(0, angle, 0) * transform.forward * test, target.transform.position), 0, maxDistance);
            maxDistance -= distanceToPlayer[i];

            Debug.DrawRay(transform.position, Quaternion.Euler(0, angle, 0) * transform.forward * maxDistance, Color.red);
            angle += 45;
            maxDistance = 5;
        }

    }
}
