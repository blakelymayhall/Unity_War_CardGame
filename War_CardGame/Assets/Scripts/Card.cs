using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Card : MonoBehaviour
{
    //======================================================================
    public Sprite faceDownSprite;
    public CardData cardData = new();
    public bool isCOM;

    //======================================================================
    private Sprite faceUpSprite;
    private SpriteRenderer spriteRenderer;
    private Deck COM_deck;
    private Deck Player_deck;
    private GameManager gameManager;
    private Player player;
    private COM com;

    //======================================================================
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        faceUpSprite = AssignFaceUpSprite(cardData);
        spriteRenderer.sprite = faceDownSprite;

        COM_deck = GameObject.Find("COM").GetComponentInChildren<Deck>();
        Player_deck = GameObject.Find("Player").GetComponentInChildren<Deck>();
        gameManager = GameObject.Find("Player").GetComponent<GameManager>();
        player = Player_deck.GetComponentInParent<Player>();
        com = COM_deck.GetComponentInParent<COM>();
    }

    //======================================================================
    void Update()
    {
        if (cardData.isFaceUp && spriteRenderer.sprite != faceUpSprite)
        {
            spriteRenderer.sprite = faceUpSprite;
        }
    }

    //======================================================================
    void OnMouseDown()
    {
        bool ignore_click =
            isCOM ||
            cardData.isFaceUp ||
            cardData != Player_deck.playedCards.Last().cardData;
        if (ignore_click)
        {
            return;
        }

        PlayCard();

        // show reshuffle button after flipping card if either player is out of cards, or if there was a draw
        // and one player doesn't have enough cards to finish a war
        bool noRemainingCards = !Player_deck.cards.Any() || !COM_deck.cards.Any();
        bool notEnoughCardsForWar = Player_deck.deck_owner.PlayerDraw() && 
            (Player_deck.cards.Count <= 2 || COM_deck.cards.Count <= 2);
        bool showReshuffle = noRemainingCards || notEnoughCardsForWar;
        if (showReshuffle)
        {
            gameManager.ActivateReshuffleButton();
        }

        // If not enough cards for war - directly assign the winner and early return
        if (notEnoughCardsForWar) 
        {
            if (Player_deck.cards.Count == COM_deck.cards.Count)
            {
                player.handOutcome = HandOutcomes.Draw;
                com.handOutcome = HandOutcomes.Draw;
            }
            else if (Player_deck.cards.Count <= 2)
            {
                player.handOutcome = HandOutcomes.Lose;
                com.handOutcome = HandOutcomes.Win;
            }
            else 
            {
                player.handOutcome = HandOutcomes.Win;
                com.handOutcome = HandOutcomes.Lose;
            }
            return;
        }
    }

    //======================================================================
    public void PlayCard()
    {
        Player_deck.playedCards.Last().cardData.isFaceUp = true;
        COM_deck.playedCards.Last().cardData.isFaceUp = true;

        int player_card_rank =
            Player_deck.playedCards.Last().cardData.cardRank;
        int COM_card_rank =
            COM_deck.playedCards.Last().cardData.cardRank;

        if (player_card_rank > COM_card_rank)
        {
            player.handOutcome = HandOutcomes.Win;
            com.handOutcome = HandOutcomes.Lose;
        }
        else if (player_card_rank == COM_card_rank)
        {
            player.handOutcome = HandOutcomes.Draw;
            com.handOutcome = HandOutcomes.Draw;
        }
        else
        {
            player.handOutcome = HandOutcomes.Lose;
            com.handOutcome = HandOutcomes.Win;
        }
    }

    //======================================================================
    public static Sprite AssignFaceUpSprite(CardData cardData)
    {
        // Build up card file name to load from assets
        string cardFileName = "Assets/Sprites/Cards/card";
        cardFileName += cardData.cardSuit.ToString();
        cardFileName += cardData.cardRank switch
        {
            11 => "J",
            12 => "Q",
            13 => "K",
            14 => "A",
            _ => cardData.cardRank.ToString(),
        };
        return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(
            cardFileName + ".png");
    }
}
