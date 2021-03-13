using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Marco's IP: 184.147.95.146
    // Dan's IP: 64.137.136.12

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

        ClampPositionToScreen();
    }


    void ClampPositionToScreen()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        float x = screenPosition.x;
        float y = screenPosition.y;;

        x = Mathf.Clamp(x, 0, Camera.main.pixelWidth);
        y = Mathf.Clamp(y, 0, Camera.main.pixelHeight);

        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(x, y, screenPosition.z));
    }
}
