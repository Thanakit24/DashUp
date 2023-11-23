using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishPlatform : MonoBehaviour
{
    [Header("Vanishing Platform")]
    public bool canVanish = true;
    
    [SerializeField] private bool isVanishing = false;
    [SerializeField] private bool isRestoring = false;
    [SerializeField] private float vanishTime;
    [SerializeField] private float vanishMaxTime;
    [SerializeField] private float restoreTime;

    [Header("Sprites")]
    private BoxCollider2D boxCol;
    [SerializeField] private BoxCollider2D triggerCol;
    private SpriteRenderer currentSprite;
    public Sprite defaultSprite;
    public Sprite vanishedSprite;

    private void Start()
    {
        boxCol = GetComponent<BoxCollider2D>();
        currentSprite = GetComponentInChildren<SpriteRenderer>();
        currentSprite.sprite = defaultSprite;
        vanishTime = vanishMaxTime;
    }
    void Update()
    {
        if (canVanish)
        {
            if (isVanishing)
            {
                vanishTime -= Time.deltaTime;
                if (vanishTime <= 0)
                {
                    triggerCol.enabled = false;
                    boxCol.enabled = false;
                    currentSprite.sprite = vanishedSprite;
                    isVanishing = false;
                    
                    StartCoroutine(UntilRestore(restoreTime));
                }
            }
        }
    }
  
    IEnumerator UntilRestore(float restoreTime)
    {
        yield return new WaitForSeconds(restoreTime);
        print("platform restore");
        triggerCol.enabled = true;
        boxCol.enabled = true;
        currentSprite.sprite = defaultSprite;
        vanishTime = vanishMaxTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!isVanishing)
                isVanishing = true;
        }
    }

    //IEnumerator UntilVanish(float vanishTime)   //Having 2 coroutines introduced platform sprite and collider flickering bugs
    //{
    //    yield return new WaitForSeconds(vanishTime);
    //    print("platform vanished");
    //    isVanishing = false;

    //    StartCoroutine(UntilRestore(restoreTime));
    //    //when vanish is complete, the platform starts restoring
    //}
}
