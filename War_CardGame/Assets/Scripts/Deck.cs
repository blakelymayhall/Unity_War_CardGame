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
    public List<Card> playedCards = new();
    public List<CardData> cards = new();

    //======================================================================
    protected Vector3 playedCardOffset;
    protected SpriteRenderer spriteRenderer;
    protected Player deck_owner;
    protected Player opponent;
    protected BurntDeck burnDeck; 

    //======================================================================
    public virtual void Start()
    {
        deck_owner = GetComponentInParent<Player>();
        burnDeck = deck_owner.transform.Find("BurntDeck").gameObject.GetComponent<BurntDeck>();
        var players = FindObjectsOfType<Player>();
        opponent = players.FirstOrDefault(p => p != deck_owner);
        spriteRenderer = GetComponent<SpriteRenderer>();

        //LoadCards();
        _Debug_LoadCards(max_cards);
        Shuffle();
        _Debug_PrintDeck(cards);
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
    void LoadCards(List<CardData> cards_to_load = null)
    {
        // If didn't get cards to load, load standard 52 card deck
        // todo -- is this terrible? This works and its short
        if (cards_to_load == null)
        {
            const int MAX_CARDS = 52;
            for (int ii = 0; ii < MAX_CARDS; ii++)
            {
                CardData card = new();
                while (cards.Any(card_in_deck => card == card_in_deck))
                {
                    card = new();
                }
                cards.Add(card);
            }
            return;
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
    public void DrawCard()
    {
        bool noCardsPlayed = playedCards.Count() == 0;
        if (noCardsPlayed)
        {
            InstantiateCard();
            return;
        }

        bool canPlayCard = playedCards.Last().cardData.isFaceUp && cards.Any();
        if (canPlayCard && deck_owner.PlayerDraw())
        {
            if (cards.Count > 2)
            {
                InstantiateCard();
                InstantiateCard();

            }
            else
            {
                // player lose
                Debug.Log("Player Lose");
            }
        }
        else if (canPlayCard && !deck_owner.PlayerDraw())
        {
            AddCardsToBurnPile();
            InstantiateCard();
        }
    }

    //======================================================================
    public void AddCardsToBurnPile()
    {
        // Handle Win
        if (deck_owner.PlayerWin())
        {
            List<Card> cardsToBurn = playedCards.Concat(
                opponent.GetComponentInChildren<Deck>().playedCards).ToList();
            burnDeck.AddCards(cardsToBurn);
        }

        // Clear Played Cards
        if (!deck_owner.PlayerDraw())
        {
            foreach (Card card in playedCards)
            {
                Destroy(card.gameObject);
            }
            playedCards.Clear();
        }
    }

    //======================================================================
    public virtual void InstantiateCard()
    {
        bool noCardsPlayed = playedCards.Count() == 0;
        if (noCardsPlayed)
        {
            playedCardOffset = new Vector3(
                2f * spriteRenderer.sprite.bounds.size.x,
                0,
                0);
        }
        else
        {
            playedCardOffset += new Vector3(
                0.25f * spriteRenderer.sprite.bounds.size.x,
                0,
                -1);
        }

        playedCards.Add(Instantiate(cardPrefab,
            transform.position + playedCardOffset,
            Quaternion.identity,
            GetComponent<Transform>()).GetComponent<Card>());
        playedCards.Last().name = "playedCard";
        playedCards.Last().cardData = cards.Last();
        playedCards.Last().isCOM = true;

        cards.RemoveAt(cards.Count - 1);
        if (!cards.Any())
        {
            spriteRenderer.sprite = null;
        }
    }
}
