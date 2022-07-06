using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAI:MonoBehaviour {

    public GameObject target;
    public float visibleRange, movementSpeed;

    Rigidbody rgbd;
    Vector3 cornerOffset;
    bool lockedOnTarget = true, foundCorner;

    void Start() {
        rgbd = GetComponent<Rigidbody>();
    }

    void Update() {

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if(distanceToTarget > visibleRange)
            return;

        if(lockedOnTarget)
            transform.LookAt(target.transform.position);

        Debug.DrawRay(transform.position, transform.forward * distanceToTarget, Color.red);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, distanceToTarget)) {
            Debug.Log(hit.collider.tag);
            if(!hit.collider.CompareTag("Player")) {
                rgbd.velocity = Vector3.zero;
                lockedOnTarget = false;
                FindCorner();
                return;
            }

            lockedOnTarget = true;
            rgbd.velocity = transform.forward * movementSpeed;
        }
    }

    void FindCorner() {
        RaycastHit hit;

        float holdAngle = transform.eulerAngles.y;
        for(float i = 0; i <= 45; i += 0.1f) {
            transform.rotation = Quaternion.Euler(0, holdAngle + i, 0);
            if(Physics.Raycast(transform.position, transform.forward, out hit)) {
                if(hit.collider.CompareTag("Player"))
                    lockedOnTarget = true;

            }
        }
    }

    void MoveTowards() {

    }
}
