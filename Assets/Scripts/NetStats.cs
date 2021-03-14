using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetStatTracker
{
    public static uint MessagesSent = 0;
    public static ulong BytesSent = 0ul;
    public static uint MessagesReceived = 0;
    public static ulong BytesReceived = 0ul;
}

public class NetStats : MonoBehaviour
{
    public GUISkin StatsSkin;

    bool mShowGUI = true;
    void Start()
    {
    }

    void OnGUI()
    {
        GUI.skin = StatsSkin;

        if (mShowGUI)
        {
            GUI.Box(new Rect(2, 2, 400, 300), "");
            GUI.Label(new Rect(10, 30, 400, 30), $"Messages Sent: {NetStatTracker.MessagesSent}");
            GUI.Label(new Rect(10, 60, 400, 30), $"Bytes Sent: {NetStatTracker.BytesSent}");
            GUI.Label(new Rect(10, 120, 400, 30), $"Messages Received: {NetStatTracker.MessagesReceived}");
            GUI.Label(new Rect(10, 150, 400, 30), $"Bytes Sent: {NetStatTracker.BytesReceived}");
            if (GUI.Button(new Rect(120, 250, 150, 40), "Hide Stats"))
                mShowGUI = false;
        }
        else
        {
            if (GUI.Button(new Rect(120, 10, 150, 40), "Show Stats"))
                mShowGUI = true;
        }
    }
}
