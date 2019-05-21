using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Part is another serializable data storage class just like WeaponDefinition
/// </summary>

[System.Serializable]
public class Part {
    // these three fields need to be defined in the Inspector pane
    public string name; // the name of this part
    public float health; // the amount of health this part has
    public string[] protectedBy; // the other parts that protect this

    // these two fields are set automatically in Start()
    // Caching like this makes it faster and eiaser to find these later
    [HideInInspector] // makes field on the next line not appear in the Inspector
    public GameObject go; // the gameObject of this part

    [HideInInspector]
    public Material mat; // The Material to show damage
}


/// <summary>
/// Enemy_4 will start offscreen and then pick a random point on screen to
///   move to. Once it has arrived, it will pick another random point and
///   continue until the player has shot it down
/// </summary>

public class Enemy_4 : Enemy {
    public Part[] parts; // The array of ship Parts

    private Vector3 p0, p1; // the two points to interpolate
    private float timeStart; // birth time for this Enemy_4
    private float duration = 4; // duration of movement

    // Start is called before the first frame update
    void Start() {
        // there is already an initial position chosen by Main.SpawnEnemy()
        //  so add it to points as the initial p0 & p1
        p0 = p1 = pos;

        InitMovement();

        // Cache GameObject & Material of each Part in parts
        Transform t;
        foreach (Part prt in parts) {
            t = transform.Find(prt.name);
            if (t != null) {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }
    void InitMovement() {
        p0 = p1; // set p0 to the old p1
        // assign a new on-screen location to p1
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        // reset the time
        timeStart = Time.time;
    }
    public override void Move() {
        // this completel overrides Enemy.Move() with a linear interpolation
        float u = (Time.time - timeStart) / duration;

        if (u >= 1) {
            InitMovement();
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2); // apply Ease Out easing to u
        pos = (1 - u) * p0 + u * p1; // simple linear interpolation

    }

    // these two functions find a PArt in parts based on name or GameObject
    Part FindPart(string n) {
        foreach(Part prt in parts) {
            if (prt.name == n) {
                return (prt);
            }
        }
        return (null);
    }
    Part FindPart(GameObject go) {
        foreach(Part prt in parts) {
            if (prt.go == go) {
                return (prt);
            }
        }
        return (null);
    }
    // these functions return true if the Part has been destroyed
    bool Destroyed(GameObject go) {
        return (Destroyed(FindPart(go)));
    }
    bool Destroyed(string n) {
        return (Destroyed(FindPart(n)));
    }
    bool Destroyed (Part prt) {
        if (prt == null) { // if no real ph was passed in
            return (true); // return true (meaning yes, it was destroyed)

        }
        // returns the result of the comparison: prt.health <= 0
        // if prt.health is 0 or less, returns true (yes, it was destroyed)
        return (prt.health <= 0);
    }

    // this changes the color of just one Part to red instead of the whole ship
    void ShowLocalizedDamage(Material m) {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    // this will override the OnCollisionEnter that is part of Enemy.cs
    void OnCollisionEnter(Collision coll) {
        GameObject other = coll.gameObject;
        switch(other.tag) {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                // if this enemy is off screen, don't damage it
                if (!bndCheck.isOnScreen) {
                    Destroy(other);
                    break;
                }
                // Hurt this Enemy
                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if (prtHit == null) { // if prtHit wasn't found
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                // Check whether this part is still protected
                if (prtHit.protectedBy != null) {
                    foreach(string s in prtHit.protectedBy) {
                        // if one of the protecting parts hasn't been destroyed...
                        if (!Destroyed(s)) {
                            // ... then don't damage this part yet
                            Destroy(other); // destroy the projectileHero
                            return; // return before damaging Enemy_4
                        }
                    }
                }
                // It's not protected, so make it take damage
                // Get the damage amount from the Projectile.type and Main.W_DEFS $$$$$$$$$
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                //show damage on the part
                ShowLocalizedDamage(prtHit.mat);
                if (prtHit.health <= 0) {
                    // instead of destroying this enemy, disable the damaged part
                    prtHit.go.SetActive(false);
                }
                // Check to see if the whole ship is destroyed
                bool allDestroyed = true; // assume it is destroyed
                foreach(Part prt in parts) {
                    if (!Destroyed(prt) ) { // if a part still exists...
                        allDestroyed = false; // .. change allDestroyed to false
                        break; // & break out of the foreach loop
                    }
                }
                if (allDestroyed) { // if it IS completely destroyed...
                    // ... tell the Main singleton that this ship was destroyed
                    Main.S.ShipDestroyed(this);
                    // Destroy this Enemy
                    Destroy(this.gameObject);
                }
                Destroy(other); // destroy the projectileHero
                break;

        }
        
    }
}
