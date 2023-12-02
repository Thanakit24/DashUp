using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotateSpeed;
    public bool rotateRight = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //rotateRight = !rotateRight;

        if (rotateRight)
            transform.Rotate(0, 0, -rotateSpeed, Space.World);
        else
            transform.Rotate(0, 0, rotateSpeed, Space.World);
    }
}
