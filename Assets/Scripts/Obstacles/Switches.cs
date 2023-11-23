using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switches : MonoBehaviour
{
    public bool activate = false;
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
            activate = !activate;

            if (activate)
            {
                activate = true;
                // open door, bridge, etc. 
            }
            else
            {
                // close door, bridge, etc
            }
        }
    }
}
