using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;

static class EndPointChooser
{
    public static readonly IPEndPoint DanEndPoint = new IPEndPoint(new IPAddress(new byte[] { 64, 137, 136, 12 }), 35353);
    public static readonly IPEndPoint MarcoEndPoint = new IPEndPoint(new IPAddress(new byte[] { 184, 147, 95, 146 }), 35353);

    public static readonly IPEndPoint ChosenEndPoint = null;

    static EndPointChooser()
    {
        if (Environment.UserName == "Dan")
        {
            ChosenEndPoint = MarcoEndPoint;
        }
        else
        {
            ChosenEndPoint = DanEndPoint;
        }
    }
}

public class PlayerController : MonoBehaviour
{
    // Marco's IP: 184.147.95.146
    // Dan's IP: 64.137.136.12

    public float Speed = 1.0f;

    private Socket mSocket = null;


    void Start()
    {
        // Marco controls the Marco GameObject... Dan controls the Dan GameObject
        if (!gameObject.name.Contains(Environment.UserName))
        {
            enabled = false;
            return;
        }

        mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        transform.Translate(horizontal * Speed * deltaTime, vertical * Speed * deltaTime, 0.0f);

        ClampPositionToScreen();

        string message = $"{transform.position.x}|{transform.position.y}";

        mSocket.SendTo(Encoding.UTF8.GetBytes(message), EndPointChooser.ChosenEndPoint);
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
