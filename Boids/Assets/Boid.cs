using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {

    //this static list holds all boid instances & is shared mongst them
    static public List<Boid> boids;

    // note: this code does NOT use a Rigid body. It handles velocity directly
    public Vector3 velocity; // current velocity
    public Vector3 newVelocity; // velocity for next frame
    public Vector3 newPosition; // position for next frame

    public List<Boid> neighbors; // all nearby Boids
    public List<Boid> collisionRisks; // All boids that are too close
    public Boid closest; // the single closest Boid

    //Initialize this Boid on Awake()
    void Awake() {
        // define the boids List if it is still null
        if (boids == null) {
            boids = new List<Boid>();
        }
        // Add this Boid to boids
        boids.Add(this);

        // Give this boid instance a random position and relocity
        Vector3 randPos = Random.insideUnitSphere * BoidSpawner.S.spawnRadius;
        randPos.y = 0; //Flatten the boid to only move in the XZ plane
        this.transform.position = randPos;
        velocity = Random.onUnitSphere;
        velocity *= BoidSpawner.S.spawnVelocity;

        // Initialize the two Lists
        neighbors = new List<Boid>();
        collisionRisks = new List<Boid>();

        // Make this.transform a child of the Boids GameObject
        this.transform.parent = GameObject.Find("Boids").transform;


        // give the Boid a random color, but make sure it's not too dark
        Color randColor = Color.black;
        while (randColor.r + randColor.g + randColor.b < 1.0f) {
            randColor = new Color(Random.value, Random.value, Random.value);
        }
        Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends) {
            r.material.color = randColor;
        }
    }

    // Update is called once per frame
    void Update() {
        // get the list of nearby Boids (this Boid's neighbours)
        List<Boid> neighbors = GetNeighbors(this);

        // Initialize newVelocity and newPosition to the current values
        newVelocity = velocity;
        newPosition = this.transform.position;

        // velocity matching: this sets the velocity of the boid to be
        // similar to that of its neighbors

        Vector3 neighborVel = GetAverageVelocity(neighbors);

        // utilizes the fields set on the boidspawner.S Singleton
        newVelocity += neighborVel * BoidSpawner.S.velocityMatchingAmt;

        // flock centering: Move towards middle of neighbours
        Vector3 neighborCenterOffset = GetAveragePosition(neighbors) - this.transform.position;
        newVelocity += neighborCenterOffset * BoidSpawner.S.flockCenteringAmt;

        // collision avoidance: avoid running into boids that are too close
        Vector3 dist;
        if (collisionRisks.Count > 0) {
            Vector3 collisionAveragePos = GetAveragePosition(collisionRisks);
            dist = collisionAveragePos - this.transform.position;
            newVelocity += dist * BoidSpawner.S.collisionAvoidanceAmt;

        }

        // mouse attraction - move towards the mouse no matter how far away
  
        dist = BoidSpawner.S.mousePos - this.transform.position;
        if (dist.magnitude > BoidSpawner.S.mouseAvoidanceDist) {
            newVelocity += dist * BoidSpawner.S.mouseAttractionAmt;
        }
        else {
            // if the mouse is too close, move away quickly!
            newVelocity -= dist.normalized * BoidSpawner.S.mouseAvoidanceDist * BoidSpawner.S.mouseAvoidanceAmt;
        }

        // newVelocity & newPosition are ready, but wait until LateUpdate()
        // to set them so that this Boid doesn't move before otehrs have
        // had a chance to calculate their new values
    }

    // by allowing all boids to update() themselves before any Boids
    // move, we avoid race conditions taht could be caused by some Boids
    // moving before others have decided where to go

    void LateUpdate() {
        // adjust the current velocity based on newVelocity using a linear
        // interpolation
        velocity = (1 - BoidSpawner.S.velocityLerpAmt) * velocity + BoidSpawner.S.velocityLerpAmt * newVelocity;

        // make sure the velocity is within min and max limits
        if (velocity.magnitude > BoidSpawner.S.maxVelocity) {
            velocity = velocity.normalized * BoidSpawner.S.maxVelocity;
        }
        if (velocity.magnitude < BoidSpawner.S.minVelocity) {
            velocity = velocity.normalized * BoidSpawner.S.minVelocity;
        }

        // decide on the new Position
        newPosition = this.transform.position + velocity * Time.deltaTime;

        // keep everything in the XZ plane
        newPosition.y = 0;

        // look from the old position at the newPosition to orient the model
        this.transform.LookAt(newPosition);

        // actually move to the new position
        this.transform.position = newPosition;


    }

    // find which boids are near enough to be considered neighbors
    // boi is BoidofInterest, the boid on which we're focusing
    public List<Boid> GetNeighbors(Boid boi) {
        float closestDist = float.MaxValue;
        Vector3 delta;
        float dist;
        neighbors.Clear();
        collisionRisks.Clear();

        foreach (Boid b in boids) {
            if (b == boi) {
                continue;
            }
            delta = b.transform.position - boi.transform.position;
            dist = delta.magnitude;
            if (dist < closestDist) {
                closestDist = dist;
                closest = b;
            }
            if (dist < BoidSpawner.S.nearDist) {
                neighbors.Add(b);
            }
            if (dist < BoidSpawner.S.collisionDist) {
                collisionRisks.Add(b);
            }

        }
        if (neighbors.Count == 0) {
            neighbors.Add(closest);
        }
        return (neighbors);
    }

    // get the average position of the Boids in a List<Boid>
    public Vector3 GetAveragePosition(List<Boid> someBoids) {
        Vector3 sum = Vector3.zero;
        foreach (Boid b in someBoids) {
            sum += b.transform.position;
        }
        Vector3 center = sum / someBoids.Count;
        return (center);
    }

    // get the average velocity of the Boids in a List<Boid>
    public Vector3 GetAverageVelocity(List<Boid> someBoids) {
        Vector3 sum = Vector3.zero;
        foreach(Boid b in someBoids) {
            sum += b.velocity;
        }
        Vector3 avg = sum / someBoids.Count;
        return (avg);
    }

}
