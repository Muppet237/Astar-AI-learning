using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAI:MonoBehaviour {

    public GameObject target;
    public float visibleRange;

    void Start() {

    }

    void Update() {


        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if(distanceToTarget > visibleRange)
            return;

        RaycastHit hit;
        Debug.DrawRay(transform.position, target.transform.position, Color.red);
        if(Physics.Raycast(transform.position, target.transform.position, out hit, 25)) {
            if(!hit.collider.CompareTag("Player"))
                return;


        }
    }

    void MoveTowards() {

    }
}
