using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour {
    // this is a singleton of the BoidSpawner. There is only one instance
    // of BoidSpawner, so we can store it in a static variable named S

    static public BoidSpawner S;

    // these fields allow you to adjust the behaviour of the Boids as a group
    public int numBoids = 250;
    public GameObject boidPrefab;
    public float spawnRadius = 100f;
    public float spawnVelocity = 10f;
    public float minVelocity = 0f;
    public float maxVelocity = 30f;
    public float nearDist = 30f;
    public float collisionDist = 5f;
    public float velocityMatchingAmt = 0.01f;
    public float flockCenteringAmt = 0.15f;
    public float collisionAvoidanceAmt = -0.5f;
    public float mouseAttractionAmt = 0.01f;
    public float mouseAvoidanceAmt = 0.75f;
    public float mouseAvoidanceDist = 15f;
    public float velocityLerpAmt = 0.25f;

    public bool __________________;

    public Vector3 mousePos;

    // Start is called before the first frame update
    void Start() {
        // set the singleton S to be this instance of BoidSpawner
        S = this;

        // instantiate numBoids (currently 100) Boids
        for (int i = 0; i < numBoids; i++) {
            Instantiate(boidPrefab);
        }
    }

    // Update is called once per frame
    void LateUpdate() {
        // track the mouse position. This keeps it all the same for all Boids
        Vector3 mousePos2d = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.transform.position.y);
        mousePos = Camera.main.ScreenToWorldPoint(mousePos2d);
    }
}
