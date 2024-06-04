using System.Net;
using System.Net.Sockets;
using NLog;

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
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
            
            try
            {
                await client.ConnectAsync(connection);
            }
            catch (SocketException ex)
            {
                Logger.Warn($"Connection to {connection} failed;");
                continue;
            }
            
            await client.SendAsync(data, SocketFlags.None);
            Logger.Info($"Sent notification to {connection}.");
        }
    }

    /// <summary>
    /// Adds a new client connection to the list of connected clients.
    /// </summary>
    /// <param name="host">host.docker.internal when running in a docker container</param>
    /// <param name="port">The port number of the client.</param>
    public static void AddConnection(string host, int port)
    {
        var ipPoint = new IPEndPoint(Dns.GetHostEntry(host).AddressList[0], port);

        // Check if the connection already exists in the list
        if (Connections.Any(c => c.Equals(ipPoint)))
        {
            Logger.Info($"Connection {ipPoint} is already subscribed");
            return;
        }

        Logger.Info($"Added new connection {ipPoint}");
        Connections.Add(ipPoint);
    }
}