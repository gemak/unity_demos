using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// this enum contains the different phases of a game turn
public enum TurnPhase {
    idle,
    pre,
    waiting,
    post,
    gameOver
}

public class Bartok : MonoBehaviour {
    static public Bartok S;
    static public Player CURRENT_PLAYER;

    [Header("Set in Inspector")]
    public TextAsset deckXML;
    public TextAsset layoutXML;
    public Vector3 layoutCenter = Vector3.zero;
    public float handFanDegrees = 10f;
    public int numStartingCards = 7;
    public float drawTimeStagger = 0.1f;

    [Header("Set Dynamically")]
    public Deck deck;
    public List<CardBartok> drawPile;
    public List<CardBartok> discardPile;
    public List<Player> players;
    public CardBartok targetCard;
    public TurnPhase phase = TurnPhase.idle;

    private BartokLayout layout;
    private Transform layoutAnchor;

    void Awake() {
        S = this;
    }
    void Start() {
        print("start");
        deck = GetComponent<Deck>(); // get the Deck
        deck.InitDeck(deckXML.text); // pass DeckXML to it
        Deck.Shuffle(ref deck.cards); // this shuffles the deck

        layout = GetComponent<BartokLayout>(); // get the layout
        layout.ReadLayout(layoutXML.text); // pass LayoutXML to it

        drawPile = UpgradeCardsList(deck.cards);
        LayoutGame();
    }
    List<CardBartok> UpgradeCardsList(List<Card> lCD) {
        List<CardBartok> lCB = new List<CardBartok>();
        foreach (Card tCD in lCD) {
            lCB.Add(tCD as CardBartok);
        }
        return (lCB);
    }
    // position all the cards in the drawPile properly
    public void ArrangeDrawPile() {
        CardBartok tCB;

        for (int i = 0; i < drawPile.Count; i++) {
            tCB = drawPile[i];
            tCB.transform.SetParent(layoutAnchor);
            tCB.transform.localPosition = layout.drawPile.pos;
            // Rotation should start at 0
            tCB.faceUp = false;
            tCB.SetSortingLayerName(layout.drawPile.layerName);
            tCB.SetSortOrder(-i * 4); // order them from front-to-back
            tCB.state = CBState.drawpile;
        }
    }
    // perform the initial game layout
    void LayoutGame() {
        print("hi layout");
        // create an empty GameObject to serve as the tableau's anchor
        if (layoutAnchor == null) {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;

        }
        //position the drawPile cards
        ArrangeDrawPile();

        // set up the players
        Player p1;
        players = new List<Player>();

        foreach (SlotDef tSD in layout.slotDefs) {
            p1 = new Player();
            p1.handSlotDef = tSD;
            players.Add(p1);
            p1.playerNum = tSD.player;
        }
        players[0].type = PlayerType.human; // make only the 0th player human

        CardBartok tCB;

        // deal seven cards to each player
        for ( int i = 0; i < numStartingCards; i++) {
            for (int j = 0; j < 4; j++) {
                tCB = Draw(); // draw a card
                // stagger the draw time a bit
                tCB.timeStart = Time.time + drawTimeStagger * (i * 4 + j);

                players[(j + 1) % 4].AddCard(tCB);
            }
        }
        Invoke("DrawFirstTarget", drawTimeStagger * (numStartingCards * 4 + 4));
    }
    public void DrawFirstTarget() {
        // flip up the first target card from the drawPile
        CardBartok tCB = MoveToTarget(Draw());

        // set the CardBartok to call CBCallback on this Bartok when it is done
        tCB.reportFinishTo = this.gameObject;
    }

    // this callb ack is used by the last card to be dealt at the beginning
    public void CBCallback(CardBartok cb ) {
        // you sometimes want to have reporting of methods calls like this
        Utils.tr("Bartok:CBCallback()", cb.name);
        StartGame(); // start the game
    }

    public void StartGame() {
        // pick the player to the left of the human to go first
        PassTurn(1);
    }

    public void PassTurn(int num = -1) {
        // if no number was passed in, pick the next player
        if (num == -1) {
            int ndx = players.IndexOf(CURRENT_PLAYER);
            num = (ndx + 1) % 4;
        }
        int lastPlayerNum = -1;
        if (CURRENT_PLAYER != null) {
            lastPlayerNum = CURRENT_PLAYER.playerNum;

            // check for Game Over and need to reshuffle discards
            if (CheckGameOver()) {
                return;
            }
        }
        CURRENT_PLAYER = players[num];
        phase = TurnPhase.pre;

        CURRENT_PLAYER.TakeTurn();

        // Report the turn passing
        Utils.tr("Bartok:PassTurn()", "Old: " + lastPlayerNum, "New: " + CURRENT_PLAYER.playerNum);
    }
    public bool CheckGameOver() {
        // see if we need to reshuffle the discard pile into the draw pile
        if (drawPile.Count == 0) {
            List<Card> cards = new List<Card>();
            foreach (CardBartok cb in discardPile) {
                cards.Add(cb);
            }
            discardPile.Clear();
            Deck.Shuffle(ref cards);
            drawPile = UpgradeCardsList(cards);
            ArrangeDrawPile();
        }
        // check to see if the current player has won
        if (CURRENT_PLAYER.hand.Count == 0) {
            // the player that just played has onw!
            phase = TurnPhase.gameOver;
            Invoke("RestartGame", 1);
            return (true);
        }
        return (false);
    }
    public void RestartGame() {
        CURRENT_PLAYER = null;
        SceneManager.LoadScene("__Bartok_Scene_0");
    }

    // ValidPlay verifies that the card chosen can be played on the discard pile
    public bool ValidPlay(CardBartok cb) {
        // it's a valid play if the rank is the same
        if (cb.rank == targetCard.rank) {
            return (true);
        }
        // it's a valid play if the suit is the same
        if (cb.suit == targetCard.suit) {
            return (true);
        }
        // Otherwise, return false
        return (false);
    }
    // this makes a new card the target
    public CardBartok MoveToTarget(CardBartok tCB) {
        tCB.timeStart = 0;
        tCB.MoveTo(layout.discardPile.pos + Vector3.back);
        tCB.state = CBState.toTarget;
        tCB.faceUp = true;

        tCB.SetSortingLayerName("10");
        tCB.eventualSortLayer = layout.target.layerName;
        if (targetCard != null) {
            MoveToDiscard(targetCard);
        }

        targetCard = tCB;

        return (tCB);
    }
    public CardBartok MoveToDiscard(CardBartok tCB) {
        tCB.state = CBState.discard;
        discardPile.Add(tCB);
        tCB.SetSortingLayerName(layout.discardPile.layerName);
        tCB.SetSortOrder(discardPile.Count * 4);
        tCB.transform.localPosition = layout.discardPile.pos + Vector3.back / 2;

        return (tCB);
    }

    // the draw function will pull a single card from the drawPile and return it
    public CardBartok Draw() {
        CardBartok cd = drawPile[0]; // pull the 0th CardBartok

        if (drawPile.Count == 0) {  // if the drawPile isn ow empty
                                    // we need to shuffle the discards into the drawPile
            int ndx; 
            while (discardPile.Count > 0) {
                // pull a random card from the discard pile
                ndx = Random.Range(0, discardPile.Count);
                drawPile.Add(discardPile[ndx]);
                discardPile.RemoveAt(ndx);
            }
            ArrangeDrawPile();
            // show the cards moving to the drawPile
            float t = Time.time;
            foreach(CardBartok tCB in drawPile) {
                tCB.transform.localPosition = layout.discardPile.pos;
                tCB.callbackPlayer = null;
                tCB.MoveTo(layout.drawPile.pos);
                tCB.timeStart = t;
                t += 0.02f;
                tCB.state = CBState.toDrawpile;
                tCB.eventualSortLayer = "0";
            }
        }
        drawPile.RemoveAt(0); // Then remove it from List<> drawPile
        return (cd); // and return it
    }
    public void CardClicked(CardBartok tCB) {
        if (CURRENT_PLAYER.type != PlayerType.human) {
            return;
        }
        if (phase == TurnPhase.waiting) {
            return;
        }
        switch(tCB.state) {
            case CBState.drawpile:
                // Draw the top card, not necessarily the one clicked
                CardBartok cb = CURRENT_PLAYER.AddCard(Draw());
                cb.callbackPlayer = CURRENT_PLAYER;
                Utils.tr("Bartok:CardClickeD()", "Draw", cb.name);
                phase = TurnPhase.waiting;
                break;
            case CBState.hand:
                // check to see whether the card is valid
                if (ValidPlay(tCB)) {
                    CURRENT_PLAYER.RemoveCard(tCB);
                    MoveToTarget(tCB);
                    tCB.callbackPlayer = CURRENT_PLAYER;
                    Utils.tr("Bartok:CardClicked()", "Play", tCB.name, targetCard.name + " is target");
                }
                else {
                    // just ignore it but report what the player tried
                    Utils.tr("Bartok:CardClicked()", "Attempted to play", tCB.name, targetCard.name + " is target");
                }
                break;

        }
    }

    // this Update() is temporarily used to test adding cards to players' hands
    //void Update() {
    //    if (Input.GetKeyDown(KeyCode.Alpha1)) {
    //        players[0].AddCard(Draw());

    //    }   
    //    if (Input.GetKeyDown(KeyCode.Alpha2)) {
    //        players[1].AddCard(Draw());
    //    }   
    //    if (Input.GetKeyDown(KeyCode.Alpha3)) {
    //        players[2].AddCard(Draw());
    //    }   
    //    if (Input.GetKeyDown(KeyCode.Alpha4)) {
    //        players[3].AddCard(Draw());
    //    }
    //}
}