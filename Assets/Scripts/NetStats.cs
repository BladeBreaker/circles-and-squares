using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct NetStatInfo
{
    public DateTime Timestamp;
    public ulong MessageLength;
}

public static class NetStatTracker
{
    public static List<NetStatInfo> MessagesReceived = new List<NetStatInfo>();
    public static List<NetStatInfo> MessagesSent = new List<NetStatInfo>();

    public static TimeSpan MessageLife = TimeSpan.FromSeconds(5);

    public static void TrackMessageReceived(ulong messageLength)
    {
        MessagesReceived.Add(new NetStatInfo() { MessageLength = messageLength, Timestamp = DateTime.Now });
        RemoveOldTrackedMessages(MessagesSent, DateTime.Now - MessageLife);
    }

    public static void TrackMessageSent(ulong messageLength)
    {
        MessagesSent.Add(new NetStatInfo() { MessageLength = messageLength, Timestamp = DateTime.Now });
        RemoveOldTrackedMessages(MessagesSent, DateTime.Now - MessageLife);
    }

    public static void RemoveOldTrackedMessages(List<NetStatInfo> messages, DateTime date)
    {
        messages.RemoveAll(message => message.Timestamp < date);
    }

    public static ulong AverageBytesPerSecond(List<NetStatInfo> messageList)
    {
        RemoveOldTrackedMessages(messageList, DateTime.Now - MessageLife);

        if (messageList.Count == 0)
            return 0ul;

        ulong bytesSent = 0ul;
        foreach (var message in messageList)
        {
            bytesSent += message.MessageLength;
        }

        double bytesSentD = (double)bytesSent;
        bytesSentD = bytesSentD / MessageLife.TotalSeconds;

        bytesSent = (ulong)bytesSentD;
        return bytesSent;
    }

    public static uint MessagesPerSecond(List<NetStatInfo> messages)
    {
        RemoveOldTrackedMessages(messages, DateTime.Now - MessageLife);

        if (messages.Count == 0)
            return 0u;

        double messageCountDouble = (double)messages.Count;
        messageCountDouble = messageCountDouble / MessageLife.TotalSeconds;

        return (uint)(messageCountDouble);
    }

    public static (uint messagesSentPerSecond, ulong bytesSentPerSecond, uint messagesReceivedPerSecond, ulong bytesReceivedPerSecond) GetNetStats()
    {
        ulong averageBytesSent = AverageBytesPerSecond(MessagesSent);
        ulong averageBytesReceived = AverageBytesPerSecond(MessagesReceived);

        uint messagesSent = MessagesPerSecond(MessagesSent);
        uint messagesReceived = MessagesPerSecond(MessagesReceived);

        return (messagesSent, averageBytesSent, messagesReceived, averageBytesReceived);
    }
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
            var (messagesSentPerSecond, bytesSentPerSecond, messagesReceivedPerSecond, bytesReceivedPerSecond) = NetStatTracker.GetNetStats();

            GUI.Box(new Rect(2, 2, 400, 300), "");
            GUI.Label(new Rect(10, 60, 400, 30), $"Sent (Bps): {bytesSentPerSecond}");
            GUI.Label(new Rect(10, 30, 400, 30), $"Sent (Messages): {messagesSentPerSecond}");
            GUI.Label(new Rect(10, 120, 400, 30), $"Received (Bps): {bytesReceivedPerSecond}");
            GUI.Label(new Rect(10, 150, 400, 30), $"Received (Messages): {messagesReceivedPerSecond}");
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
