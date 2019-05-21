using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;   // the speed in m/s
    public float fireRate = 0.3f;   // seconds/shot (unused)
    public float health = 10;
    public int score = 100;     // points earned for destroying this
    private BoundsCheck bndCheck;

    void Awake() {
        bndCheck = GetComponent<BoundsCheck>();   
    }

    // this is a property: a method that acts like a field
    public Vector3 pos {
        get {
            return (this.transform.position);
        }
        set {
            this.transform.position = value;
        }
    }

    // Update is called once per frame
    void Update() {
        Move();

        if (bndCheck != null && bndCheck.offDown) {
            // check to make sure its's gone off the bottom of teh screen
            if (pos.y < bndCheck.camHeight - bndCheck.radius) {
                // we're off the bottom, so destroy this game ovbject
                Destroy(gameObject);
            }
        }
    }
    public virtual void Move() {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    void OnCollisionEnter(Collision coll) {
        GameObject otherGO = coll.gameObject;
        if (otherGO.tag == "ProjectileHero") {
            Destroy(otherGO); // destroy the projectile
            Destroy(gameObject);  // destroy this Enemy GameObject
        }
        else {
            print("Enemy hit by non-ProjectileHero: " + otherGO.name);
        }
    }
}
