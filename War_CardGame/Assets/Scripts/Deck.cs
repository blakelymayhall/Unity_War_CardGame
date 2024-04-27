using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    //======================================================================
    public int max_cards;
    public int number_of_cards;

    //======================================================================
    private List<Card> cards = new();

    //======================================================================
    void Start()
    {
        
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
}
