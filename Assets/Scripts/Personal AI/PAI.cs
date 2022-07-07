using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAI:MonoBehaviour {

    public GameObject target, pointer;
    public float visibleRange, movementSpeed, bodyToWall = 1;

    Rigidbody rgbd;
    Vector3 cornerOffset, tempVector;
    bool lockedOnPlayer = true, foundCorner, findPlayer;
    float distanceToTarget;

    void Start() {
        rgbd = GetComponent<Rigidbody>();
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit)) {
            tempVector = hit.point;
        }
    }

    void Update() {
        Debug.DrawRay(transform.position, transform.forward * distanceToTarget, Color.red);

        distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if(distanceToTarget > visibleRange)
            return;

        if(findPlayer) {
            FindPlayer();
            return;
        }

        if(foundCorner) {
            if(Vector3.Distance(cornerOffset, transform.position) < 0.1f) {
                foundCorner = false;
                findPlayer = true;
                return;
            }

            MoveTowards(cornerOffset);
            return;
        }

        if(lockedOnPlayer)
            StandardMovement();
    }

    void FindPlayer() {
        transform.LookAt(target.transform.position);

        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, distanceToTarget)) {
            if(hit.collider.CompareTag("Player")) {
                lockedOnPlayer = true;
                findPlayer = false;
                return;
            }
        }
        tempVector = hit.point;
        findPlayer = false;
        FindCorner();
    }

    void StandardMovement() {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, distanceToTarget)) {
            if(!hit.collider.CompareTag("Player")) {
                rgbd.velocity = Vector3.zero;
                tempVector = hit.point;
                lockedOnPlayer = false;
                FindCorner();
                return;
            }
        }
        MoveTowards(target.transform.position);
    }

    void FindCorner() {
        float holdAngle = transform.eulerAngles.y;
        
        RaycastHit hitFind;
        for(float i = 1; i <= 90; i += 0.25f) {
            transform.rotation = Quaternion.Euler(0, holdAngle + i, 0);
            if(Physics.Raycast(transform.position, transform.forward, out hitFind)) {
                if(Vector3.Distance(tempVector, hitFind.point) > 0.1) {
                    cornerOffset = tempVector + Vector3.right * bodyToWall;
                    foundCorner = true;
                    Debug.Log(cornerOffset);
                    //Instantiate(pointer, cornerOffset, Quaternion.identity);
                    break;
                }
                tempVector = hitFind.point;
            }
        }
    }

    void MoveTowards(Vector3 _target) {
        //Debug.Log(_target);
        transform.LookAt(_target);
        rgbd.velocity = transform.forward * movementSpeed;
    }
}
