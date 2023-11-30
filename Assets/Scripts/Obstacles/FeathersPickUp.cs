using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeathersPickUp : MonoBehaviour
{
    [SerializeField]
    public bool isActive = true;
    public float restoreTime;
    public int poopToIncrease;
    //[SerializeField] private bool isGoldFeather;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //IEnumerator UntilRestore(float restoreTime)
    //{
    //    yield return new WaitForSeconds(restoreTime);
    //    print("platform restore");
    //    if (!isActive)
    //    {
    //        isActive = true;
    //    }
    //    //set back the sprite to its original sprite


    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isActive)
        {
            {
                print("picked up bread");
                PlayerController player = collision.GetComponent<PlayerController>();
                player.amountOfPoop += poopToIncrease;
                isActive = false;
                Destroy(gameObject);
            }
            //play some effects or particles and sounds
            //isActive = false;
            //StartCoroutine(UntilRestore(restoreTime));
            //change sprite to restoring/transparent sprite;
            //deactive this instead
        }
    }
}
