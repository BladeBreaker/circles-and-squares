using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float Speed = 1.0f;

    void Start()
    {

    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        transform.Translate(horizontal * Speed * deltaTime, vertical * Speed * deltaTime, 0.0f);
    }
}
