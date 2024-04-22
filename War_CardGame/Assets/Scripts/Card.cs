using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public enum CardSuit {
        Clubs,
        Spades,
        Hearts,
        Diamonds
    }

    private bool isFaceUp;
    private int cardRank;
    private CardSuit cardSuit;
    private Sprite faceUpSprite;
    private Sprite faceDownSprite;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Randomize which suit
        cardSuit = AssignSuit();
        // Randomize which value
        cardRank =  Random.Range(2, 14);
        // Assign the sprites
        spriteRenderer.sprite = // load the sprite from the folder by name
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    CardSuit AssignSuit()
    {
        System.Array values = System.Enum.GetValues(typeof(CardSuit));
        int randomIndex = UnityEngine.Random.Range(0, values.Length);
        return (CardSuit)values.GetValue(randomIndex);
    }
}
