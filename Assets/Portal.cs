using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public bool firstPortal = true;
    public Transform portalA;
    public Transform portalB;
    public bool poopInPortal = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //play animation or particles
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Poop") && !poopInPortal)
        {
            if (firstPortal)
            {
                poopInPortal = true;
                collision.transform.position = portalB.position;
            }
            else
            {
                collision.transform.position = portalA.position;
                poopInPortal = true;
            } 
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Poop") && poopInPortal)
        {
            poopInPortal = false;

        }
    }
}
