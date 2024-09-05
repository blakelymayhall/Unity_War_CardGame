using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    //======================================================================
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        faceUpSprite = AssignFaceUpSprite(cardData);
        spriteRenderer.sprite = faceDownSprite;

        COM_deck = GameObject.Find("COM").GetComponentInChildren<Deck>();
        Player_deck = GameObject.Find("Player").GetComponentInChildren<Deck>();
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
        Debug.Log(Player_deck.playedCards.Last().cardData.cardRank);
        bool ignore_click =
            isCOM ||
            cardData.isFaceUp ||
            cardData != Player_deck.playedCards.Last().cardData;
        if (ignore_click)
        {
            return;
        }

        PlayCard();
    }

    //======================================================================
    public void PlayCard() 
    {
        Player_deck.playedCards.Last().cardData.isFaceUp = true;
        COM_deck.playedCards.Last().cardData.isFaceUp = true;

        Player player = Player_deck.GetComponentInParent<Player>();
        COM com = COM_deck.GetComponentInParent<COM>();

        int player_card_rank =
            Player_deck.playedCards.Last().cardData.cardRank;
        int COM_card_rank =
            COM_deck.playedCards.Last().cardData.cardRank;

        if (player_card_rank > COM_card_rank)
        {
            player.handOutcome = HandOutcomes.Win;
            com.handOutcome = HandOutcomes.Lose;
            Debug.Log("Player Wins");
        }
        else if (player_card_rank == COM_card_rank)
        {
            player.handOutcome = HandOutcomes.Draw;
            com.handOutcome = HandOutcomes.Draw;
            Debug.Log("Draw");
        }
        else 
        {
            player.handOutcome = HandOutcomes.Lose;
            com.handOutcome = HandOutcomes.Win;
            Debug.Log("COM Win");
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
