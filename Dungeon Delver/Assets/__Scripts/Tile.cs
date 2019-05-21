using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    [Header("Set Dynamically")]
    public int x;
    public int y;
    public int tileNum;

    private BoxCollider bColl;

    void Awake() {
        bColl = GetComponent<BoxCollider>();
    }

    public void SetTile(int eX, int eY, int eTileNum = -1) {
        x = eX;
        y = eY;
        transform.localPosition = new Vector3(x, y, 0);
        gameObject.name = x.ToString("D3") + "x" + y.ToString("D3");

        if (eTileNum == -1) {
            eTileNum = TileCamera.GET_MAP(x, y);
        }
        else {
            TileCamera.SET_MAP(x, y, eTileNum); // repalce if non-default tileNum
        }
        tileNum = eTileNum;
        GetComponent<SpriteRenderer>().sprite = TileCamera.SPRITES[tileNum];

        SetCollider();
    }

    // Arrange the collider for this tile
    void SetCollider() {
        // Collider info is pulled from DelverCollisions.txt
        bColl.enabled = true;
        char c = TileCamera.COLLISIONS[tileNum];
        switch(c) {
            case 'S': // whole
                bColl.center = Vector3.zero;
                bColl.size = Vector3.one;
                break;
                
            case 'W': // whole
                bColl.center = new Vector3(0, 0.25f, 0);
                bColl.size = new Vector3(1, 0.5f, 1);
                break;
                
            case 'A': // whole
                bColl.center = new Vector3(-0.25f, 0, 0);
                bColl.size = new Vector3(0.5f, 1, 1);
                break;
                
            case 'D': // whole
                bColl.center = new Vector3(0.25f, 0, 0);
                bColl.size = new Vector3(0.5f, 1, 1);
                break;
                
            case 'Q': // whole
                bColl.center = new Vector3(0.25f, 0.25f, 0);
                bColl.size = new Vector3(0.5f, 0.5f, 1);
                break;
                
            case 'E': // whole
                bColl.center = new Vector3(0.25f, 0.25f, 0);
                bColl.size = new Vector3(0.5f, 0.5f, 1);
                break;
                
            case 'Z': // whole
                bColl.center = new Vector3(-0.25f, -0.25f, 0);
                bColl.size = new Vector3(0.5f, 0.5f, 1);
                break;
                
            case 'X': // whole
                bColl.center = new Vector3(0, -0.25f, 0);
                bColl.size = new Vector3(1, 0.5f, 1);
                break;
                
            case 'C': // whole
                bColl.center = new Vector3(0.25f, -0.25f, 0);
                bColl.size = new Vector3(0.5f, 0.5f, 1);
                break;

            default: // anything else: _, |, etc.
                bColl.enabled = false;
                break;
        }
    }
}
