using UnityEngine;
using System.Collections;

public class TurnLight: MonoBehaviour {
    void Update() {
        transform.position = Vector3.back * 3;
        
        if (Bartok.CURRENT_PLAYER == null) {
            return;
        }
        transform.position += Bartok.CURRENT_PLAYER.handSlotDef.pos;
    }
}