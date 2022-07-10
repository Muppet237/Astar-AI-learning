using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAI:MonoBehaviour {

    public GameObject target, pointer, cornerPointer;
    public float visibleRange, movementSpeed, bodyToWall = 1, bodyThickness = 1;

    GameObject testObject;

    Rigidbody rgbd;
    Vector3 cornerOffset, tempVectorRight, tempVectorLeft;
    bool lockedOnPlayer = true, foundCorner, findPlayer;
    float distanceToTarget, marginDistance, distanceBetweenPoints;

    void Start() {
        Application.targetFrameRate = 120;
        rgbd = GetComponent<Rigidbody>();
        transform.LookAt(target.transform.position);
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
        Debug.DrawRay(transform.position + transform.right * 0.5f, transform.forward * distanceToTarget, Color.red);
        //return;

        if(distanceToTarget > visibleRange)
            return;

        if(findPlayer) {
            FindPlayer();
            return;
        }

        if(foundCorner) {
            MoveTowardsCorner();
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
        tempVectorRight = tempVectorLeft = hit.point;
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
                    tempVectorRight = tempVectorLeft = hit.point;
                    lockedOnPlayer = false;
                    FindCorner();
                    return;
                }
            }
        }
        MoveTowards(target.transform.position);
    }

    void MoveTowardsCorner() {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, Vector3.Distance(transform.position, cornerOffset))) {
            if(Vector3.Distance(transform.position, hit.point) < 5) {
                foundCorner = false;
                findPlayer = true;
                return;
            }
        }

        if(Vector3.Distance(cornerOffset, transform.position) < 0.2f) {
            foundCorner = false;
            findPlayer = true;
            return;
        }

        MoveTowards(cornerOffset);
    }

    void FindCorner() {
        float holdAngle = transform.eulerAngles.y;

        RaycastHit hitFind;
        for(float i = 0.25f; i <= 90; i += 0.25f) {
            transform.rotation = Quaternion.Euler(0, holdAngle + i, 0);
            //yield return new WaitForSeconds(0.05f);
            if(Physics.Raycast(transform.position, transform.forward, out hitFind)) {
                //Instantiate(pointer, hitFind.point, Quaternion.identity);
                Debug.Log(Vector3.Distance(tempVectorRight, hitFind.point));
                if(Vector3.Distance(tempVectorRight, hitFind.point) > 1) {
                    transform.LookAt(tempVectorRight + transform.right * 0.5f);
                    transform.rotation = Quaternion.Euler(0, holdAngle + i + bodyToWall, 0);
                    cornerOffset = transform.position + (transform.forward * 1.1f) * Vector3.Distance(transform.position, tempVectorRight);
                    Instantiate(pointer, cornerOffset, Quaternion.identity);
                    foundCorner = true;
                    break;
                }

                if(i >= 0.5f) {
                    if(Vector3.Distance(tempVectorRight, hitFind.point) < distanceBetweenPoints) {
                        break;
                    }
                }
                //If the distance between tempVector and hit.point becomes smaller, it means that the AI is standing close to a wall or an already used corner
                //So break the check and move to the left hand check
                distanceBetweenPoints = Vector3.Distance(tempVectorRight, hitFind.point);
                tempVectorRight = hitFind.point;
            } else {
                if(Vector3.Distance(tempVectorRight, hitFind.point) > 1) {
                    transform.LookAt(tempVectorRight + transform.right * 0.5f);
                    transform.rotation = Quaternion.Euler(0, holdAngle + i + bodyToWall, 0);
                    cornerOffset = transform.position + (transform.forward * 1.1f) * Vector3.Distance(transform.position, tempVectorRight);
                    Instantiate(pointer, cornerOffset, Quaternion.identity);
                    foundCorner = true;
                    break;
                }
            }
        }

        if(foundCorner)
            return;

        for(float i = 0.25f; i <= 270; i += 0.25f) {
            transform.rotation = Quaternion.Euler(0, holdAngle - i, 0);
            //yield return new WaitForSeconds(0.05f);
            if(Physics.Raycast(transform.position, transform.forward, out hitFind)) {
                //Instantiate(pointer, hitFind.point, Quaternion.identity);
                if(Vector3.Distance(tempVectorLeft, hitFind.point) > 1) {
                    transform.LookAt(tempVectorLeft - transform.right * 0.5f);
                    transform.rotation = Quaternion.Euler(0, holdAngle - i - bodyToWall, 0);
                    cornerOffset = transform.position + (transform.forward * 1.1f) * Vector3.Distance(transform.position, tempVectorLeft);
                    Instantiate(pointer, cornerOffset, Quaternion.identity);
                    foundCorner = true;
                    break;
                }
                marginDistance = Vector3.Distance(tempVectorLeft, hitFind.point);
                tempVectorLeft = hitFind.point;
            } else {
                if(Vector3.Distance(tempVectorLeft, hitFind.point) > 1) {
                    transform.LookAt(tempVectorLeft - transform.right * 0.5f);
                    transform.rotation = Quaternion.Euler(0, holdAngle - i - bodyToWall, 0);
                    cornerOffset = transform.position + (transform.forward * 1.1f) * Vector3.Distance(transform.position, tempVectorLeft);
                    Instantiate(pointer, cornerOffset, Quaternion.identity);
                    foundCorner = true;
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
