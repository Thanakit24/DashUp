using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform destination;
    private CircleCollider2D col; // Reference to the collider
    [SerializeField] private float disabledDuration;

    private void Awake()
    {
        // Get the collider component
        col = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Poop"))
        {
            PoopBehavior poop = collision.GetComponent<PoopBehavior>();
           
            poop.DisableTrail();
            if (Vector2.Distance(collision.transform.position, transform.position) > 0.4f)
            {
                AudioManager.instance.Play("Teleport");
                collision.transform.position = destination.transform.position;
                poop.EnableTrail();
                StartCoroutine(DisableColliderForDuration(disabledDuration)); // Change 2f to your desired duration
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Poop"))
        {
            PoopBehavior poop = collision.GetComponent<PoopBehavior>();
            poop.EnableTrail();
        }
    }
    private IEnumerator DisableColliderForDuration(float duration)
    {
        // Disable the collider
        col.enabled = false;

        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Enable the collider after the duration
        col.enabled = true;
    }
}
