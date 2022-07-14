using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayPAI:MonoBehaviour {

    public GameObject target;
    public float speed, pushSpeed;

    float[] rayDistance = new float[8];
    float maxDistance = 10;

    Rigidbody rgbd;

    void Start() {
        rgbd = GetComponent<Rigidbody>();
    }

    void Update() {
        transform.LookAt(target.transform.position);
        Vector3 forcedDirection = target.transform.position - transform.position;

        int angle = 0;
        for(int i = 0; i < 8; i++) {
            float distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);
            rayDistance[i] = Mathf.Clamp(Vector3.Distance(transform.position + Quaternion.Euler(0, angle, 0) * transform.forward * distanceToPlayer, target.transform.position), 0, maxDistance);
            maxDistance -= rayDistance[i] * 0.5f;

            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * transform.forward;

            Debug.DrawRay(transform.position, rayDirection * maxDistance, Color.red);

            RaycastHit hit;
            if(Physics.Raycast(transform.position, rayDirection * maxDistance, out hit)) {
                if(!hit.collider.CompareTag("Player")) {
                    if(Vector3.Distance(transform.position, hit.point) < 3) {
                        rgbd.velocity = (Vector3.one + -rayDirection.normalized * pushSpeed) + forcedDirection.normalized * speed;
                    } else {
                        rgbd.velocity = forcedDirection.normalized * speed;
                    }
                } else {
                    rgbd.velocity = forcedDirection.normalized * speed;
                }
            }
            angle += 45;
            maxDistance = 10;
        }
    }
}
