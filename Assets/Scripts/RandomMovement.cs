using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement:MonoBehaviour {

    public float speed, raidus; 

    void Update() {
        transform.position = new Vector3(Mathf.Cos(Time.time * speed) * raidus, transform.position.y, Mathf.Sin(Time.time * speed) * raidus);
    }
}
