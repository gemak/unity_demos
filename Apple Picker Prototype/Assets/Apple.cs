using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour {

    [Header("Set in Inspector")]
    public static float bottomY = -40f;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update() {
        if (transform.position.y < bottomY) {
            Destroy(this.gameObject);

            // get a reference to the ApplePicker compoenent of MainCamera
            ApplePicker apScript = Camera.main.GetComponent<ApplePicker>();

            // calle the public AppleDestroyed() method of apScript
            apScript.AppleDestroyed();
        }
    }
}
