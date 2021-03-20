using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using static EndPointChooser;

static class EndPointChooser
{
    public static readonly int GamePort = 35353;
    public static readonly int NatPTServerPort = 35357;

    public static IPAddress NatPTServerAddress = IPAddress.Parse("64.137.136.12");

    // Necessary because Dan can't hit clone-god on public address
    public static IPAddress DanNatPTServerAddress = IPAddress.Parse("10.88.111.200");

    public static readonly IPEndPoint NatPTServerEndpoint = null;

    public static readonly IPEndPoint NoneEndpoint = new IPEndPoint(IPAddress.Loopback, 55555);

    public static Socket sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

    public static EndPoint LastKnownEndpoint = null;

    static EndPointChooser()
    {
        if (Environment.UserName == "Dan")
        {
            NatPTServerAddress = DanNatPTServerAddress;
        }

        NatPTServerEndpoint = new IPEndPoint(NatPTServerAddress, NatPTServerPort);
    }
}

public class PlayerController : MonoBehaviour
{
    public float Speed = 1.0f;
    public TimeSpan TickRate = TimeSpan.FromMilliseconds(34);

    private DateTime mLastDataSentTimeStamp = DateTime.MinValue;


    void Start()
    {
        // Marco controls the Marco GameObject... Dan controls the Dan GameObject
        if (!gameObject.name.Contains(Environment.UserName))
        {
            enabled = false;
            return;
        }

        byte[] bytes = Encoding.UTF8.GetBytes("balls");
        sSocket.SendTo(bytes, NatPTServerEndpoint);
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
        if (DateTime.Now >= mLastDataSentTimeStamp + TickRate && LastKnownEndpoint != null)
        {
            string message = $"{transform.position.x}|{transform.position.y}";

            Debug.Log($"Sending to Endpoint: {LastKnownEndpoint}");

            byte[] bytes = Encoding.UTF8.GetBytes(message);
            sSocket.SendTo(bytes, EndPointChooser.LastKnownEndpoint);

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
