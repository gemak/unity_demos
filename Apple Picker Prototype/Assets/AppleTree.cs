using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleTree : MonoBehaviour {

    //prefab for instantiating apples
    public GameObject applePrefab;

    // Speed at which the apple tree moves in meters/second
    public float speed = 10f;

    // distance where appletree turns around
    public float leftAndRightEdge = 25f;

    // Chance that the appletree will change direction
    public float chanceToChangeDirections = 0.1f;

    // rate at which apples will be instantiated
    public float secondsBetweenAppleDrops = 1f;

    // Start is called before the first frame update
    void Start() {
        // dropping apples every second
        InvokeRepeating("DropApple", 2f, secondsBetweenAppleDrops);
    }
    
    void DropApple() {
        GameObject apple = Instantiate(applePrefab) as GameObject;
        apple.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update() {
        // basic movement
        Vector3 pos = transform.position;
        pos.x += speed * Time.deltaTime;
        transform.position = pos;

        // changing direction
        if (pos.x < -leftAndRightEdge) {
            speed = Mathf.Abs(speed); // move right
        }
        else if (pos.x > leftAndRightEdge) {
            speed = -Mathf.Abs(speed); // move left
        }
    }
    void FixedUpdate() {
        // changing direction randomly
        if (Random.value < chanceToChangeDirections) {
            speed *= -1; // change direction
        }
    }
}
