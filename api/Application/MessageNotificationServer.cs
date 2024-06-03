using System.Net;
using System.Net.Sockets;

namespace api.Application;

/// <summary>
/// This class provides methods for managing and notifying clients about messages.
/// </summary>
public static class MessageNotificationServer
{
    /// <summary>
    /// A list of connected client IP Endpoints.
    /// </summary>
    private static readonly List<IPEndPoint> Connections = new();

    /// <summary>
    /// Notifies all connected clients about a new message.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>
    /// Adds a new client connection to the list of connected clients.
    /// </summary>
    /// <param name="ip">The IP address of the client.</param>
    /// <param name="port">The port number of the client.</param>
    public static void AddConnection(string ip, int port)
    {
        var ipPoint = new IPEndPoint(IPAddress.Parse(ip).MapToIPv6(), port);

        // Check if the connection already exists in the list
        if (Connections.Any(c => c.Equals(ipPoint)))
            return;

        Connections.Add(ipPoint);
    }
}