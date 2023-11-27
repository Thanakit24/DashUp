using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeathersPickUp : MonoBehaviour
{
    [SerializeField] 
    private int poopToIncrease;
    [SerializeField] private bool isGoldFeather;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            isGoldFeather = !isGoldFeather;

            if (isGoldFeather)
            {
                player.amountOfJumps = player.maxAmountOfJumps;
            }
            else
            {
                print("picked up bread");
                player.amountOfPoop += poopToIncrease;
            }
            //play some effects or particles and sounds
            Destroy(gameObject);
        }
    }
}
