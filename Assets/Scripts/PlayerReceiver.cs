using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class PlayerReceiver : MonoBehaviour
{
    private Socket mSocket = null;

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.name.Contains(Environment.UserName))
        {
            enabled = false;
            return;
        }

        mSocket = new Socket(EndPointChooser.ChosenOpponentEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

        mSocket.Bind(EndPointChooser.LocalBindEndPoint);
    }

    // Update is called once per frame
    void Update()
    {
        while (mSocket.Available > 0)
        {
            byte[] buffer = new byte[15000];
            EndPoint endpoint = EndPointChooser.ChosenOpponentEndPoint;

            mSocket.ReceiveFrom(buffer, ref endpoint);

            if (endpoint == EndPointChooser.MarcoEndPoint)
            {
                Debug.Log("Baller, it's marco");
            }

            int stringLen = 0;
            for (int i = 0; i < buffer.Length; ++i)
            {
                if (buffer[i] == 0)
                {
                    stringLen = i;
                    break;
                }
            }

            string message = Encoding.UTF8.GetString(buffer, 0, stringLen);

            NetStatTracker.MessagesReceived++;
            NetStatTracker.BytesReceived += (ulong)stringLen;

            Debug.Log($"Message: {message}");

            string[] coords = message.Split('|');

            float x = float.Parse(coords[0]);
            float y = float.Parse(coords[1]);

            transform.position = new Vector3(x, y, transform.position.z);
        }
    }
}
