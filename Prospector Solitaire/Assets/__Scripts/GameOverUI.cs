using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // required for the uGUI classes like Text

public class GameOverUI : MonoBehaviour {
    private Text txt;

    void Awake() {
        txt = GetComponent<Text>();
        txt.text = "";
    }
    void Update() {
        if (Bartok.S.phase != TurnPhase.gameOver) {
            txt.text = "";
            return;
        }    
        // we only get here if the game is over
        if (Bartok.CURRENT_PLAYER == null) {
            return;
        }
        if (Bartok.CURRENT_PLAYER.type == PlayerType.human) {
            txt.text = "You won!";
        }
        else {
            txt.text = "Game Over";
        }
    }
}