using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;


public class Deck : MonoBehaviour
{
    //======================================================================
    public int max_cards;
    public GameObject cardPrefab;
    public List<Card> playedCards = new();
    public List<CardData> cards = new();
    public Player deck_owner;
    public Player opponent;
    public BurntDeck burnDeck; 
    public GameObject cardCount;
    public DeckAnimator deckAnimator;
    public SpriteRenderer spriteRenderer;
    public Sprite faceDownSprite;
    //======================================================================
    protected Vector3 playedCardOffset;
    protected GameManager gameManager;
    //======================================================================

    public virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = GameObject.Find("Player").GetComponent<GameManager>();
        playedCardOffset = new Vector3(
                2f * spriteRenderer.sprite.bounds.size.x,
                deck_owner.isCOM ? -1f : 1f,
                0);

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

        var tm = cardCount.GetComponent<TextMeshProUGUI>();
        tm.text = "Cards:\n" + cards.Count;
    }

    //======================================================================
    public void LoadCards(List<CardData> cards_to_load = null)
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
        else 
        {
            foreach (CardData cardData in cards_to_load)
            {
                cards.Add(cardData);
            }
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
        deckAnimator.StartShuffle();
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
        if(!GetComponentInParent<Deck>().deck_owner.isCOM)
            gameManager.DisableDrawButton();
            
        if (playedCards.Count() == 0)
        {
            InstantiateCard();
        }
        else if (cards.Count >= 2)
        {
            // Only other way to get here is if there was a draw on the 
            // previous hand, and both players have sufficient cards to war
            InstantiateCard();
            InstantiateCard();
        }

        gameManager.UpdateCardCounter(cardCount, cards.Count);
    }

    //======================================================================
    List<Card> GetCardsToBurn()
    {
        List<Card> cardsToBurn = new(); 
        if (deck_owner.PlayerWin())
        {
            cardsToBurn = playedCards.Concat(opponent.GetComponentInChildren<Deck>().playedCards).ToList();
            
        }
        else if (deck_owner.PlayerPush())
        {
            cardsToBurn = playedCards;
        }
        
        return cardsToBurn;
    }

    //======================================================================
    public void AddCardsToBurnPile(bool finalCards = false)
    {
        List<Card> cardsToBurn = GetCardsToBurn();
        burnDeck.AddCards(GetCardsToBurn());
        if (!finalCards)
        {
            foreach (Card card in cardsToBurn)
            {
                card.cardAnimator.StartMoveCard(burnDeck.gameObject.transform.position, 20f, true);
            }
        }
    }

    //======================================================================
    public void ClearPlayedCards()
    {
        if (!deck_owner.PlayerDraw())
        {
            playedCards.Clear();
        }
    }

    //======================================================================
    public void ResetAfterRound()
    {
        if (deck_owner.PlayerWin() || deck_owner.PlayerPush())
        {
            List<Card> cardsToMove = GetCardsToBurn();
            foreach (Card card in cardsToMove)
            {
                card.cardAnimator.StartMoveCard(transform.position, 20f, true);
            }
        }

        foreach (CardData cardData in burnDeck.cards)
        {
            cardData.isFaceUp = false;
            cards.Add(cardData);
        }

        burnDeck.cards.Clear();
        spriteRenderer.sprite = faceDownSprite;
        burnDeck.GetComponent<SpriteRenderer>().sprite = null;
        gameManager.UpdateCardCounter(cardCount, cards.Count);
        if(!GetComponentInParent<Deck>().deck_owner.isCOM)
            gameManager.ActivateDrawButton();
        Shuffle();
    }

    //======================================================================
    public void ResetAfterMatch()
    {
        burnDeck.cards.Clear();
        burnDeck.GetComponent<SpriteRenderer>().sprite = null;
        spriteRenderer.sprite = faceDownSprite;
        cards.Clear();
        foreach (Card card in playedCards)
        {
            Destroy(card.gameObject);
        }
        playedCards.Clear();
        _Debug_LoadCards(max_cards);
        Shuffle();
        gameManager.ActivateDrawButton();
        gameManager.UpdateCardCounter(cardCount, cards.Count);
    }

    //======================================================================
    public void InstantiateCard()
    {
        Vector3 destination;
        if (playedCards.Count() == 0)
        {
            destination = playedCardOffset;
        }
        else
        {
            destination = playedCardOffset + new Vector3(0.25f*playedCards.Count(), 0, -1);
        }

        Card newCard = Instantiate(cardPrefab,
            transform.position,
            Quaternion.identity,
            GetComponent<Transform>()).GetComponent<Card>();
        newCard.name = "playedCard";
        newCard.cardData = cards.Last();
        newCard.isCOM = deck_owner.isCOM;
        newCard.cardAnimator.StartMoveCard(transform.position+destination, 10f);
        playedCards.Add(newCard);

        cards.RemoveAt(cards.Count - 1);
        if (!cards.Any())
        {
            spriteRenderer.sprite = null;
        }
    }
}
