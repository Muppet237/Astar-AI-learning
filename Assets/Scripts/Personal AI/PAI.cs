using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAI:MonoBehaviour {

    public GameObject target, pointer;
    public float visibleRange, movementSpeed, bodyToWall = 1, bodyThickness = 1;

    Rigidbody rgbd;
    Vector3 cornerOffset, tempVectorRight, tempVectorLeft;
    bool lockedOnPlayer = true, foundCorner, findPlayer;
    float distanceToTarget, marginDistance;

    void Start() {
        Application.targetFrameRate = 120;
        rgbd = GetComponent<Rigidbody>();
        transform.LookAt(target.transform.position);
        Debug.DrawRay(transform.position, transform.forward, Color.blue);
        Debug.DrawRay(transform.position - transform.right * 0.5f, transform.forward, Color.red);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit)) {
            tempVectorRight = hit.point;
            tempVectorLeft = hit.point;
        }
        //StartCoroutine(FindCorner());
    }

    void Update() {
        distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        Debug.DrawRay(transform.position, transform.forward * distanceToTarget, Color.blue);
        Debug.DrawRay(transform.position - transform.right * 0.5f, transform.forward * distanceToTarget, Color.red);
        //return;

        if(distanceToTarget > visibleRange)
            return;

        if(findPlayer) {
            FindPlayer();
            return;
        }

        if(foundCorner) {
            if(Vector3.Distance(cornerOffset, transform.position) < 0.2f) {
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
            } else {
                if(Vector3.Distance(transform.position, hit.point) > 5) {
                    MoveTowards(hit.point);
                    return;
                }
            }
        }
        tempVectorRight = hit.point;
        findPlayer = false;
        FindCorner();
    }

    void StandardMovement() {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, distanceToTarget)) {
            if(!hit.collider.CompareTag("Player")) {
                //Debug.Log(Vector3.Distance(transform.position, hit.point));
                if(Vector3.Distance(transform.position, hit.point) > 5) {
                    MoveTowards(hit.point);
                } else {
                    rgbd.velocity = Vector3.zero;
                    tempVectorRight = hit.point;
                    lockedOnPlayer = false;
                    FindCorner();
                    return;
                }
            }
        }
        MoveTowards(target.transform.position);
    }

    void FindCorner() {
        float holdAngle = transform.eulerAngles.y;

        RaycastHit hitFind, hitFindLeft;
        for(float i = 0.25f; i <= 90; i += 0.25f) {
            transform.rotation = Quaternion.Euler(0, holdAngle + i, 0);
            //yield return new WaitForSeconds(0.05f);
            if(Physics.Raycast(transform.position, transform.forward, out hitFind)) {
                //Instantiate(pointer, hitFind.point, Quaternion.identity);
                if(Vector3.Distance(tempVectorRight, hitFind.point) > 0.1) {
                    for(float x = 0.25f; x <= 45; x += 0.25f) {
                        transform.rotation = Quaternion.Euler(0, holdAngle + i + x, 0);
                        //yield return new WaitForSeconds(0.25f);
                        if(Physics.Raycast(transform.position - transform.right * bodyThickness, transform.forward, out hitFindLeft)) {
                            //Debug.Log(Vector3.Distance(tempVectorRight, hitFindLeft.point));
                            if(Vector3.Distance(hitFindLeft.point, tempVectorRight) < marginDistance) {
                                transform.rotation = Quaternion.Euler(0, holdAngle + i + x + bodyToWall, 0);
                                cornerOffset = transform.position + (transform.forward * 1.1f) * Vector3.Distance(transform.position, tempVectorRight);
                                Instantiate(pointer, cornerOffset, Quaternion.identity);
                                foundCorner = true;
                                break;
                            }
                        }
                    }
                    break;
                }
                marginDistance = Vector3.Distance(tempVectorRight, hitFind.point);
                tempVectorRight = hitFind.point;
            } else {
                if (Vector3.Distance(tempVectorRight, hitFind.point) > 0.1) {
                    for(float x = 0.25f; x <= 45; x += 0.25f) {
                        transform.rotation = Quaternion.Euler(0, holdAngle + i + x, 0);
                        //yield return new WaitForSeconds(0.25f);
                        if(Physics.Raycast(transform.position - transform.right * bodyThickness, transform.forward, out hitFindLeft)) {
                            //Debug.Log(Vector3.Distance(tempVectorRight, hitFindLeft.point));
                            if(Vector3.Distance(hitFindLeft.point, tempVectorRight) < marginDistance) {
                                transform.rotation = Quaternion.Euler(0, holdAngle + i + x + bodyToWall, 0);
                                cornerOffset = transform.position + (transform.forward * 1.1f) * Vector3.Distance(transform.position, tempVectorRight);
                                Instantiate(pointer, cornerOffset, Quaternion.identity);
                                foundCorner = true;
                                break;
                            }
                        }
                    }
                    break;
                }
            }
        }
    }

    void MoveTowards(Vector3 _target) {
        //Debug.Log(_target);
        transform.LookAt(_target);
        rgbd.velocity = transform.forward * movementSpeed;
    }
}
