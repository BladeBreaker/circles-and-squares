using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;

static class EndPointChooser
{
    public static readonly int port = 35353;

    public static readonly IPEndPoint DanEndPoint = new IPEndPoint(IPAddress.Parse("64.137.136.12"), port);
    public static readonly IPEndPoint MarcoEndPoint = new IPEndPoint(IPAddress.Parse("184.147.95.146"), port);
    public static readonly IPEndPoint LocalBindEndPoint = new IPEndPoint(IPAddress.Any, port);

    public static readonly IPEndPoint ChosenOpponentEndPoint = null;

    static EndPointChooser()
    {
        if (Environment.UserName == "Dan")
        {
            ChosenOpponentEndPoint = MarcoEndPoint;
        }
        else
        {
            ChosenOpponentEndPoint = DanEndPoint;
        }
    }
}

public class PlayerController : MonoBehaviour
{
    // Marco's IP: 184.147.95.146
    // Dan's IP: 64.137.136.12

    public float Speed = 1.0f;
    public TimeSpan TickRate = TimeSpan.FromMilliseconds(34);

    private Socket mSocket = null;

    private DateTime mLastDataSentTimeStamp = DateTime.MinValue;


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

        TrySendPositionData();
    }

    private void TrySendPositionData()
    {
        if (DateTime.Now >= mLastDataSentTimeStamp + TickRate)
        {
            string message = $"{transform.position.x}|{transform.position.y}";

            byte[] bytes = Encoding.UTF8.GetBytes(message);
            mSocket.SendTo(bytes, EndPointChooser.ChosenOpponentEndPoint);

            NetStatTracker.TrackMessageSent((ulong)(bytes.Length * sizeof(byte)));

            if (DateTime.Now - mLastDataSentTimeStamp > TimeSpan.FromMilliseconds(200))
            {
                mLastDataSentTimeStamp = DateTime.Now;
            }
            else
            {
                mLastDataSentTimeStamp += TickRate;
            }
        }
    }


    void ClampPositionToScreen()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        float x = screenPosition.x;
        float y = screenPosition.y;

        x = Mathf.Clamp(x, 0, Camera.main.pixelWidth);
        y = Mathf.Clamp(y, 0, Camera.main.pixelHeight);

        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(x, y, screenPosition.z));
    }
}
