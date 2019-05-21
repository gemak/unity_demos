using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {
    [Header("Set in Inspector")]
    public float rotationsPerSecond = .1f;

    [Header("Set Dynamically")]
    public int levelShown = 0;

    // this non-public variable will not appear in the inspector
    Material mat;


    // Start is called before the first frame update
    void Start() {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update() {
        // read the current shield level from the hero singleton
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
        // if this is different from levelShown...
        if (levelShown != currLevel) {
            levelShown = currLevel;
            // adjust the texture offset to show different shield elvel
            mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);

        }
        // rotate the shield a bit every fram in a time-based way
        float rZ = -(rotationsPerSecond * Time.time * 3600) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}
