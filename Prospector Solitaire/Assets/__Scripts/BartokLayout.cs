using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotDef {
    public float x;
    public float y;
    public bool faceUp = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public List<int> hiddenBy = new List<int>(); // unused in Bartok
    public float rot; // rotation of hands
    public string type = "slot";
    public Vector2 stagger;
    public int player; // player number of a hand
    public Vector3 pos; // pos derived from x, y, & multiplier

}

public class BartokLayout : MonoBehaviour {
    [Header("Set Dynamically")]
    public PT_XMLReader xmlr; // just lilke Deck, this has a PT_XMLReader
    public PT_XMLHashtable xml; // this variable is for faster xml access
    public Vector2 multiplier; // sets the spacing of the tableau
    //SlotDef references
    public List<SlotDef> slotDefs; // The SlotDefs hands
    public SlotDef drawPile;
    public SlotDef discardPile;
    public SlotDef target;

    // Bartok calls this method to read in the BartokLayoutXML.xml file
    public void ReadLayout(string xmlText) {
        xmlr = new PT_XMLReader();
        xmlr.Parse(xmlText); // The XML is parsed
        xml = xmlr.xml["xml"][0]; // and xml is set as a shortcut to the xml

        // read in the multiplier, which sets card spacing
        multiplier.x = float.Parse(xml["multiplier"][0].att("x"));
        multiplier.y = float.Parse(xml["multiplier"][0].att("y"));

        // read in the slots
        SlotDef tSD;
        // slotsx is used as a shortcut to all the <slot>s
        PT_XMLHashList slotsX = xml["slot"];

        for (int i = 0; i < slotsX.Count; i++) {
            tSD = new SlotDef(); // create a new slotdef instance
            if (slotsX[i].HasAtt("type")) {
                // if this <slot> has a type attribute parse it
                tSD.type = slotsX[i].att("type");
            }
            else {
                // if not, set its type to "slot"; it's a card in the rows
                tSD.type = "slot";
            }

            // various attribute are parsed into numerical values
            tSD.x = float.Parse(slotsX[i].att("x"));
            tSD.y = float.Parse(slotsX[i].att("y"));
            tSD.pos = new Vector3(tSD.x * multiplier.x, tSD.y * multiplier.y, 0);

            // sorting layers
            tSD.layerID = int.Parse(slotsX[i].att("layer"));
            tSD.layerName = tSD.layerID.ToString();

            // pull additional attrubtes based on the type of each <slot>
            switch (tSD.type) {
                case "slot":
                    // ignore slots that are just of the "slot" type
                    break;
                case "drawpile":
                    tSD.stagger.x = float.Parse(slotsX[i].att("xstagger"));
                    drawPile = tSD;
                    break;
                case "discardpile":
                    discardPile = tSD;
                    break;
                case "target":
                    target = tSD;
                    break;
                case "hand":
                    tSD.player = int.Parse(slotsX[i].att("player"));
                    tSD.rot = float.Parse(slotsX[i].att("rot"));
                    slotDefs.Add(tSD);
                    break;
            }



        }
    }

}