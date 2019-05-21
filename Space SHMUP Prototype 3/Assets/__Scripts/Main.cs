using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Main : MonoBehaviour {
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies; // array of enemy prefabs
    public float enemySpawnPerSecond = 0.5f; // # enemies/second
    public float enemyDefaultPadding = 1.5f; // padding for position
    public WeaponDefinition[] weaponDefinitions;

    private BoundsCheck bndCheck;


    void Awake() {
        S = this;
        // set bndCheck to reference the BoundsCheck component on this GameObject
        bndCheck = GetComponent<BoundsCheck>();

        // invoke SpawnEnemy() once (in 2 seconds, based on default values)
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        // a generic Dictionary with WeaponType as the key
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in weaponDefinitions) {
            WEAP_DICT[def.type] = def;
        }
    }
    public void SpawnEnemy() {
        // pick a random Enemy prefab to instantiate
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        // position the enemy above the screen with a random x position
        float enemyPadding = enemyDefaultPadding; 
        if (go.GetComponent<BoundsCheck>() != null) {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        // set the initial position for the spawned Enemy
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        //invoke spawnEnemy() again
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    public void DelayedRestart( float delay ) {
        // invoke the Restart() method in delay seconds
        Invoke("Restart", delay);
    }
    public void Restart() {
        // reload _Scene_0 to restart the game
        SceneManager.LoadScene("_Scene_0");
    }
    /// <summary>
    /// Static function that get a WeaponDefinition from the WEAP_DICT static
    /// protected field of the Main class.
    /// </summary>
    /// <returns>The WeaponDefinition or, if theres is no WEaponDefinition with
    /// the WeaponType passed in, returns a new WeaponDefinition with a
    /// WeaponType of none..</returns>
    /// <param name="wt">The WeaponType of the desired WeaponDefinition</param>
    static public WeaponDefinition GetWeaponDefinition( WeaponType wt) {
        // Chek to make sure that the key exists in the Dictionary
        // Attempting to retrive a key that didn't exist, would throw an error,
        // so the followin if statement is important
        if (WEAP_DICT.ContainsKey(wt)) {
            return (WEAP_DICT[wt]);
        }

        // this returns a new WEaponDefinition with a type of WeaponType.none,
        //  which means it has failed to find the right WeaponDefinition
        return (new WeaponDefinition());
    }
}
