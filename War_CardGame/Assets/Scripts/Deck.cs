using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    //======================================================================
    public int max_cards;
    public GameObject cardPrefab;
    public List<CardData> cards = new();
    public List<CardData> burntCards = new();

    //======================================================================
    protected Vector3 playedCardOffset;
    protected SpriteRenderer spriteRenderer;
    protected Card playedCard;
    protected Player deck_owner;
    protected Player opponent;
    //======================================================================
    public virtual void Start()
    {
        deck_owner = GetComponentInParent<Player>();
        var players = FindObjectsOfType<Player>();
        opponent = players.FirstOrDefault(p => p != deck_owner);
        spriteRenderer = GetComponent<SpriteRenderer>();

        playedCardOffset = new Vector3(
            2f*spriteRenderer.sprite.bounds.size.x,
            0,
            0);

        _Debug_LoadCards(max_cards);
        Shuffle();
        _Debug_PrintDeck(cards);
    }

    //======================================================================
    void Update()
    {

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
    void _Debug_PrintDeck(List<CardData> cards)
    {
        foreach (CardData card in cards)
        {
            Debug.Log(card.cardSuit);
            Debug.Log(card.cardRank);
            Debug.Log("--");
        }
    }

    //======================================================================
    public void Shuffle()
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
    public void PlayCard()
    {
        bool canPlayCard =
            playedCard == null ||
            playedCard.cardData.isFaceUp &&
            cards.Any();

        if (canPlayCard)
        {
            if (playedCard != null)
            {
                if (deck_owner.handOutcome == HandOutcomes.Win)
                {
                    burntCards.Add(playedCard.cardData);
                    burntCards.Add(
                        opponent.GetComponentInChildren<Deck>().
                        playedCard.cardData);
                }

                Destroy(playedCard.gameObject);
            }

            InstantiateCard();
            cards.RemoveAt(cards.Count - 1);
            if (!cards.Any())
            {
                spriteRenderer.sprite = null;
            }

            Debug.Log("Burn Cards\n\n");
            _Debug_PrintDeck(burntCards);
        }
    }

    //======================================================================
    public virtual void InstantiateCard()
    {
        playedCard = Instantiate(cardPrefab,
            transform.position + playedCardOffset,
            Quaternion.identity,
            GetComponent<Transform>()).GetComponent<Card>();
        playedCard.name = "playedCard";
        playedCard.cardData = cards.Last();
        playedCard.isCOM = true;
    }
}
