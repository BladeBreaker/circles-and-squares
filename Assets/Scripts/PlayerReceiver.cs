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
        mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    // Update is called once per frame
    void Update()
    {
        byte[] buffer = new byte[15000];
        EndPoint endpoint = null;

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

        Debug.Log($"Message: {message}");
    }
}
