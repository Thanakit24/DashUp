using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public Transform player;
    public GameObject tutorialBox;
    public GameObject exclamationBox;
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI textToDisplay;
    public bool isFacingRight = true;
    private SpriteRenderer sprite;

    private void Awake()
    {
        tutorialBox.SetActive(false);
        exclamationBox.SetActive(true);
    }
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            // Get the direction from the NPC to the player
            Vector3 directionToPlayer = player.position - transform.position;

            // Flip the sprite based on the direction
            if (directionToPlayer.x < 0 && !isFacingRight)
            {
                isFacingRight = true;
                sprite.flipX = false;
                // Player is in the positive X axis, flip right
               
            }
            else if (directionToPlayer.x > 0 && isFacingRight)
            {
                // Player is in the negative X axis, flip left
                isFacingRight = false;
                sprite.flipX = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
         if (collision.gameObject.CompareTag("Player"))
        {
            exclamationBox.SetActive(false);
            tutorialBox.SetActive(true);
            tutorialText.text = textToDisplay.text;
        }
    }
   

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            tutorialBox.SetActive(false);
            exclamationBox.SetActive(true);

        }
    }
}
