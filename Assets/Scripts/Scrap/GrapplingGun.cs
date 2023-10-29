using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    public Camera m_camera;
    [Header("Prefabs")]
    public GameObject hookPrefab;

    [Header("Gun Transform Refs")]
    public Transform player;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)] [SerializeField] private float rotationSpeed = 4;

    public float shootForce;
    [SerializeField] private bool shotHook = false; 
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        RotateGun(mousePos, true);
        

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShootHook();
        }
    }

    void ShootHook()
    {  //fire hook projectile from the fire point; 
        shotHook = true;
        GameObject hook = Instantiate(hookPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = hook.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.right * shootForce, ForceMode2D.Impulse); 
    }

    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
        {
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
        {
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}