using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using static EndPointChooser;

public class PlayerReceiver : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (gameObject.name.Contains(Environment.UserName))
        {
            enabled = false;
            return;
        }

        sSocket.Bind(new IPEndPoint(IPAddress.Any, PingServer.Port));
    }

    // Update is called once per frame
    void Update()
    {
        while (sSocket.Available > 0)
        {
            byte[] buffer = new byte[15000];
            EndPoint endpoint = NoneEndpoint;

            sSocket.ReceiveFrom(buffer, ref endpoint);

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

            if (LastKnownEndpoint == null)
            {
                string[] splits = message.Split(':');
                LastKnownEndpoint = new IPEndPoint(IPAddress.Parse(splits[0]), port);

                sSocket.Shutdown(SocketShutdown.Both);
                sSocket.Close();

                sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                sSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            }


            Debug.Log($"Endpoint: {endpoint}, message: {message}");

            NetStatTracker.TrackMessageReceived((ulong)stringLen);

            return;
 

            //Debug.Log($"Message: {message}");

            string[] coords = message.Split('|');

            float x = float.Parse(coords[0]);
            float y = float.Parse(coords[1]);

            transform.position = new Vector3(x, y, transform.position.z);
        }
    }
}
