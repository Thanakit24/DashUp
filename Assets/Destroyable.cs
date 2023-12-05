using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public bool isActive = true;
    [SerializeField] private SpriteRenderer currentSprite;
    public Sprite activeSprite;
    public Sprite deactiveSprite;
    private BoxCollider2D boxCol;
    [SerializeField] private BoxCollider2D triggerCol;
    public float restoreTime;
    // Start is called before the first frame update
    void Start()
    {
        currentSprite = GetComponent<SpriteRenderer>();
        boxCol = GetComponent<BoxCollider2D>();
        currentSprite.sprite = activeSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator UntilRestore(float restoreTime)
    {
        yield return new WaitForSeconds(restoreTime);
        print("platform restore");
        if (!isActive)
        {
            isActive = true;
            //play expand animation on restore for feedback? 
            boxCol.enabled = true;
            triggerCol.enabled = true;
            currentSprite.sprite = activeSprite;
            currentSprite.color = new Color32(255, 255, 255, 255);
            //currentSprite.color = new Color(255, 255, 255);

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Poop") && isActive)
        {
            isActive = false;
            currentSprite.sprite = deactiveSprite;
            currentSprite.color = new Color32(255, 255, 255, 50);
            //currentSprite.color = new Color32(90, 90, 90, 255);
            //play explode particles? 
            boxCol.enabled = false;
            triggerCol.enabled = false;
            StartCoroutine(UntilRestore(restoreTime));
        }
    }
  

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player") && isActive)
    //    {
    //        print("")
    //        collision.GetComponent<PlayerController>().Dead();
    //        print("player dead");
    //    }
    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player") && isActive)
    //    {
    //        print("player detect in trigger col");
    //        collision.GetComponent<PlayerController>().Dead();
    //        print("player dead");
    //    }
    //}
}
