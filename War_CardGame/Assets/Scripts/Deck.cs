using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    //======================================================================
    public int max_cards;
    public int number_of_cards;

    public List<CardData> cards = new();
    //======================================================================


    //======================================================================
    void Start()
    {
        max_cards = 3;
        number_of_cards = max_cards;

        _Debug_LoadCards(max_cards);
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
}
