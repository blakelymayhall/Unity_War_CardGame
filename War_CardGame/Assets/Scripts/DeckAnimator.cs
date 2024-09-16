using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class DeckAnimator : MonoBehaviour
{
    //======================================================================
    public GameObject cardPrefab;

    //======================================================================
    private List<Card> shuffleCards = new();
    private bool isShuffling = false;
    private float startTime;
    private GameManager gameManager;

    private const int numCards = 4;
    private const float animationDuration = 1.2f;
    private const float rotationSpeed = 4f;
    //======================================================================

    //======================================================================
    void Start()
    {
        gameManager = GameObject.Find("Player").GetComponent<GameManager>();
    }

    //======================================================================
    void Update()
    {
        if(isShuffling)
        {
            Shuffle();
        }
    }

    //======================================================================
    public void StartShuffle()
    {
        for(int ii = 0; ii < numCards; ii++)
        {
            Card newCard = Instantiate(cardPrefab,
                transform.position,
                Quaternion.Euler(0,0,Random.Range(0, 360)),
                GetComponent<Transform>()).GetComponent<Card>();
            shuffleCards.Add(newCard);
        }
        isShuffling = true;
        startTime = Time.time;
        GetComponent<Deck>().spriteRenderer.sprite = null;
        Debug.Log(gameManager);
        if(!GetComponentInParent<Deck>().deck_owner.isCOM)
            gameManager.DisableDrawButton();
    }

    //======================================================================
    void Shuffle()
    {
        if((Time.time - startTime) < animationDuration)
        {
            for (int ii = 0; ii < shuffleCards.Count/2; ii++)
            {
                shuffleCards[ii].transform.Rotate(0,0,rotationSpeed);
            }
            for (int ii = shuffleCards.Count/2; ii < shuffleCards.Count; ii++)
            {
                shuffleCards[ii].transform.Rotate(0,0,-rotationSpeed);
            }
            
        }
        else if (!AllCardsZerod())
        {
            foreach(Card card in shuffleCards) 
            {
                card.transform.rotation = Quaternion.RotateTowards(
                                            card.transform.rotation, 
                                            Quaternion.Euler(Vector3.zero), 
                                            rotationSpeed * 100f* Time.deltaTime);
            }
        }
        else 
        {
            isShuffling = false;
            gameManager.ActivateDrawButton();
            foreach (Card card in shuffleCards)
            {
                Destroy(card.gameObject);
            }
            shuffleCards.Clear();
            GetComponent<Deck>().spriteRenderer.sprite = 
                GetComponent<Deck>().cardPrefab.GetComponent<Card>().faceDownSprite;
        }
    }

    //======================================================================
    bool AllCardsZerod()
    {
        foreach(Card card in shuffleCards) 
        {
            if (Quaternion.Angle(card.transform.rotation, Quaternion.Euler(Vector3.zero)) > 0.05f)
            {
                return false;
            }
        }
        return true;
    }
}