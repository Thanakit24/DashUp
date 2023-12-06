using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeathersPickUp : MonoBehaviour
{
    [SerializeField]
    public bool isActive = true;
    public float restoreTime;
    public int poopToIncrease;
    public SpriteRenderer sprite;
    //public Sprite deactiveSprite;
    private Animator anim;
    public PlayerController player;
    //private BoxCollider2D boxCol;
    [SerializeField] private bool isGoldFeather;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        //boxCol = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        player = GameManager.instance.pc;
    }

    IEnumerator UntilRestore(float restoreTime)
    {
        yield return new WaitForSeconds(restoreTime);
        //print("platform restore");
        if (!isActive)
        {
            isActive = true;
            sprite.color = new Color(255, 255, 255);
            anim.enabled = true;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Poop")) && isActive && player.amountOfPoop == 0)
        {
            //isGoldFeather = !isGoldFeather;
            //player = collision.GetComponent<PlayerController>();

            if (!isGoldFeather)
            {
                //print("picked up bread");

                player.amountOfPoop += poopToIncrease;
                isActive = false;

            }
            else
            {
                if (player.currentEnergy > player.maxEnergy)
                    player.currentEnergy = player.maxEnergy;
                print("set player energy to full");
                isActive = false;
            }
            //play some effects or particles and sounds
            AudioManager.instance.Play("Bread");
            isActive = false;
            //player = null;
            sprite.color = new Color32(90, 90, 90, 255);
            anim.enabled = false;
            StartCoroutine(UntilRestore(restoreTime));
            //change sprite to restoring/transparent sprite;
            //deactive this instead
        }

    }
}

