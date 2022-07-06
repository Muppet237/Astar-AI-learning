using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAI:MonoBehaviour {

    public GameObject target;
    public float visibleRange, movementSpeed;

    Rigidbody rgbd;

    void Start() {
        rgbd = GetComponent<Rigidbody>();
    }

    void Update() {
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if(distanceToTarget > visibleRange)
            return;

        transform.LookAt(target.transform.position);

        //Debug.DrawRay(transform.position, transform.forward * distanceToTarget, Color.red);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, distanceToTarget)) {
            if(!hit.collider.CompareTag("Player")) {
                rgbd.velocity = Vector3.zero;
            }

            rgbd.velocity = transform.forward * movementSpeed;
        }
    }

    void MoveTowards() {

    }
}
