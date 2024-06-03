using System.Net;
using System.Net.Sockets;

namespace api.Application;

public static class MessageNotificationServer
{
    private static readonly List<IPEndPoint> Connections = new();
    
    public static async Task NotifyClients()
    {
        foreach (var connection in Connections)
        {
            using var client = new Socket(connection.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var data = "onSend"u8.ToArray();
            await client.ConnectAsync(connection);
            await client.SendAsync(data, SocketFlags.None);
        }
    }
    
    public static void AddConnection(string ip, int port)
    {
        var ipPoint = new IPEndPoint(IPAddress.Parse(ip).MapToIPv6(), port);
        
        if (Connections.Any(c => c.Equals(ipPoint)))
            return;
        
        Connections.Add(ipPoint);
    }
}