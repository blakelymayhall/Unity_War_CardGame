using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CardAnimator : MonoBehaviour
{
   
    //======================================================================
    private Card card;
    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;
    private Quaternion quat_rot_1 = Quaternion.Euler(0, rotation_part1, 0);
    private Quaternion quat_rot_2 = Quaternion.Euler(0, rotation_part2, 0);
    private Vector3 moveDestination;
    private bool isMoving = false;
    private bool destroyAfterMove = false;
    private bool isFlipping = false;

    private float moveSpeed = 10f;
    private const float rotationSpeed = 1000f;
    private const float rotation_part1 = 90f;
    private const float rotation_part2 = 180f;

    //======================================================================
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        card = GetComponentInParent<Card>();
        gameManager = GameObject.Find("Player").GetComponent<GameManager>();
    }

    void Update()
    {
        if (isFlipping)
        {
            FlipCards();
        }

        if (isMoving)
        {
            MoveCard();
        }
    }

    public void StartFlipCard()
    {
        isFlipping = true;
    }

    void FlipCards() 
    {
        if(Quaternion.Angle(transform.rotation, quat_rot_1) > 0.1f && transform.eulerAngles.y < 90)
        {
            gameManager.com.deck.playedCards.Last().transform.rotation = 
                Quaternion.RotateTowards(transform.rotation, quat_rot_1, rotationSpeed * Time.deltaTime);
            transform.rotation = 
                Quaternion.RotateTowards(transform.rotation, quat_rot_1, rotationSpeed * Time.deltaTime);
            return;
        }
        
        if (spriteRenderer.sprite == card.faceDownSprite)
        {
            spriteRenderer.sprite = card.faceUpSprite;
            spriteRenderer.flipX = true;
            gameManager.com.deck.playedCards.Last().GetComponent<SpriteRenderer>().sprite = 
                gameManager.com.deck.playedCards.Last().faceUpSprite;
            gameManager.com.deck.playedCards.Last().GetComponent<SpriteRenderer>().flipX = true;
        }

        if(Quaternion.Angle(transform.rotation, quat_rot_2) > 0.1f)
        {
            gameManager.com.deck.playedCards.Last().transform.rotation = 
                Quaternion.RotateTowards(transform.rotation, quat_rot_2, rotationSpeed * Time.deltaTime);
            transform.rotation = 
                Quaternion.RotateTowards(transform.rotation, quat_rot_2, rotationSpeed * Time.deltaTime);
            return;
        }
        
        card.cardData.isFaceUp = true;
        isFlipping = false;
    }

    public void StartMoveCard(Vector3 destination, float cardMoveSpeed, bool destroyAfter = false)
    {
        moveSpeed = cardMoveSpeed;
        moveDestination = destination;
        isMoving = true;
        destroyAfterMove = destroyAfter;
    }

    void MoveCard()
    {
        transform.position = Vector3.MoveTowards(transform.position, moveDestination, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, moveDestination) < 0.05f)
        {
            isMoving = false;
            if (destroyAfterMove)
            {
                Destroy(gameObject);
                gameManager.player.burntDeck.UpdateSprite();
                gameManager.com.burntDeck.UpdateSprite();
            }
        }
    }
}