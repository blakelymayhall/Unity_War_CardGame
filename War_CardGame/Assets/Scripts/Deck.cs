using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    //======================================================================
    public int max_cards;
    public GameObject cardPrefab;
    public List<CardData> cards = new();

    //======================================================================
    private Vector3 playedCardOffset;
    private SpriteRenderer spriteRenderer;
    private Card playedCard;

    //======================================================================
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        playedCardOffset = new Vector3(
            2f*spriteRenderer.sprite.bounds.size.x,
            0,
            0);
        max_cards = 3;

        _Debug_LoadCards(max_cards);
        Shuffle();
        _Debug_PrintDeck();
    }

    //======================================================================
    void Update()
    {

    }

    //======================================================================
    void OnMouseDown()
    {
        PlayCard();

        if (!cards.Any())
        {
            spriteRenderer.sprite = null;
        }
    }

    //======================================================================
    // Loads random cards into the deck
    void _Debug_LoadCards(int max_cards)
    {
        for (int ii = 0; ii < max_cards; ii++)
        {
            CardData card = new();
            while (cards.Any(card_in_deck => card == card_in_deck))
            {
                card = new();
            }
            cards.Add(card);
        }
    }

    //======================================================================
    void _Debug_PrintDeck()
    {
        foreach (CardData card in cards)
        {
            Debug.Log(card.cardSuit);
            Debug.Log(card.cardRank);
            Debug.Log("--");
        }
    }

    //======================================================================
    void Shuffle()
    {
        System.Random random = new System.Random();
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            CardData value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }

    //======================================================================
    void PlayCard()
    {
        bool canPlayCard =
            playedCard == null ||
            playedCard.cardData.isFaceUp &&
            cards.Any();

        if (canPlayCard)
        {
            if (playedCard != null)
            {
                Destroy(playedCard.gameObject);
            }

            playedCard = Instantiate(cardPrefab,
                transform.position + playedCardOffset,
                Quaternion.identity,
                GetComponent<Transform>()).GetComponent<Card>();
            playedCard.name = "playedCard";
            playedCard.cardData = cards.Last();

            cards.RemoveAt(cards.Count - 1);
        }
    }
}
