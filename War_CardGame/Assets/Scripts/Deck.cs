using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    //======================================================================
    public int max_cards;

    public List<CardData> cards = new();
    //======================================================================


    //======================================================================
    void Start()
    {
        max_cards = 3;

        _Debug_LoadCards(max_cards);
        Shuffle();
    }

    //======================================================================
    void Update()
    {

    }

    //======================================================================
    void OnMouseDown()
    {
        Debug.Log("Clicked on Deck");
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
        _Debug_PrintDeck();
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
        _Debug_PrintDeck();
    }
}
