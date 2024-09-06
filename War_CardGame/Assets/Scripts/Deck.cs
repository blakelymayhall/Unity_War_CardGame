using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    //======================================================================
    public int max_cards;
    public GameObject cardPrefab;
    public List<Card> playedCards = new();
    public List<CardData> cards = new();
    public Player deck_owner;
    public Player opponent;

    //======================================================================
    protected Vector3 playedCardOffset;
    protected SpriteRenderer spriteRenderer;
    protected BurntDeck burnDeck; 
    protected GameManager gameManager;

    //======================================================================
    public virtual void Start()
    {
        deck_owner = GetComponentInParent<Player>();
        burnDeck = deck_owner.transform.Find("BurntDeck").gameObject.GetComponent<BurntDeck>();
        var players = FindObjectsOfType<Player>();
        opponent = players.FirstOrDefault(p => p != deck_owner);
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = GameObject.Find("Player").GetComponent<GameManager>();

        //LoadCards();
        _Debug_LoadCards(max_cards);
        Shuffle();
    }

    //======================================================================
    void OnMouseDown()
    {
        Debug.Log(cards.Count());
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
    public void _Debug_PrintDeck(List<CardData> cards)
    {
        foreach (CardData card in cards)
        {
            Debug.Log(card.cardRank + " " + card.cardSuit);
        }
        Debug.Log("==");
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

        // Only other way to get here is if there was a draw
        if (cards.Count >= 2 && opponent.GetComponentInChildren<Deck>().cards.Count >= 2)
        {
            InstantiateCard();
            InstantiateCard();
        }
    }

    //======================================================================
    public void AddCardsToBurnPile(bool finalCards = false)
    {
        if (deck_owner.PlayerWin())
        {
            List<Card> cardsToBurn = playedCards.Concat(
                opponent.GetComponentInChildren<Deck>().playedCards).ToList();
            burnDeck.AddCards(cardsToBurn);
        }

        if (finalCards) 
        {
            if (deck_owner.PlayerDraw())
            {
                List<Card> cardsToBurn = playedCards;
                burnDeck.AddCards(cardsToBurn);
            }
        }
    }

    //======================================================================
    public void ClearPlayedCards(bool finalCards = false)
    {
        if (!deck_owner.PlayerDraw() || finalCards)
        {
            foreach (Card card in playedCards)
            {
                Destroy(card.gameObject);
            }
            playedCards.Clear();
        }
    }

    //======================================================================
    public void ResetAfterRound()
    {
        foreach (CardData cardData in burnDeck.cards)
        {
            cardData.isFaceUp = false;
            cards.Add(cardData);
        }
        burnDeck.cards.Clear();
        Shuffle();
        spriteRenderer.sprite = cardPrefab.GetComponent<SpriteRenderer>().sprite;
        burnDeck.GetComponent<SpriteRenderer>().sprite = null;
        gameManager.ActivateDrawButton();
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
