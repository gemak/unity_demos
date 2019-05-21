using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode {
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour {
    static private MissionDemolition S; // a private singleton

    [Header("Set in Inspector")]
    public Text uitLevel; // the UIText_Level Text
    public Text uitShots; // the UIText_Shots Text
    public Text uitButton; // The Text on UIButton_View
    public Vector3 castlePos; // the place to put castles
    public GameObject[] castles; // an array of the castles

    [Header("Set Dynamically")]

    public int level; // the current level
    public int levelMax; // the number of levels
    public int shotsTaken;
    public GameObject castle; // the current castle
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot"; // FollowCam mode

    // Start is called before the first frame update
    void Start() {
        S = this; // define the singleton

        level = 0;
        levelMax = castles.Length;
        StartLevel();

    }
    void StartLevel() {
        //get rid of the old castle if one exists
        if (castle != null) {
            Destroy(castle);
        }

        // destroy old projectiles if they exist
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
        foreach(GameObject pTemp in gos) {
            Destroy(pTemp);
        }

        // instantiate the new castle
        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;
        shotsTaken = 0;

        // reset the camera
        SwitchView("Show Both");
        ProjectileLine.S.Clear();

        // reset the goal
        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;
    }

    void UpdateGUI() {
        //show the data in the GUITexts
        uitLevel.text = "Level: " + (level + 1) + "of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken;


    }
    
    // Update is called once per frame
    void Update() {
        UpdateGUI();

        if ( (mode == GameMode.playing) && Goal.goalMet) {
            mode = GameMode.levelEnd;
            // zoom out
            SwitchView("Show Both");
            // start the next level in 2 seconds
            Invoke("NextLevel", 2f);
        }

    }

    void NextLevel() {
        level++;
        if (level == levelMax) {
            level = 0;
        }
        StartLevel();
    }

    public void SwitchView(string eView = "") {
        if (eView == "") {
            eView = uitButton.text;
        }
        showing = eView;
        switch (showing) {
            case "Show Slingshot":
                FollowCam.POI = null;
                uitButton.text = "Show Castle";
                break;
            case "Show Castle":
                FollowCam.POI = S.castle;
                uitButton.text = "Show Both";
                break;
            case "Show Both":
                FollowCam.POI = GameObject.Find("ViewBoth");
                uitButton.text = "Show Slingshot";
                break;
        }
    }

    // static metho that allows code anywhere to increment shotsTaken
    public static void ShotFired() {
        S.shotsTaken++;
    }
}
