using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace Receiver;

/// <summary>
/// This class represents a receiver that connects to a server, registers, waits for notifications, and displays messages.
/// </summary>
public class Receiver
{
    private readonly HttpClient _httpClient;
    private readonly string _registerUrl;
    private readonly string _receiveUrl;
    private readonly IPAddress _ip;
    private readonly int _port;

    /// <summary>
    /// Initializes a new instance of the <see cref="Receiver"/> class.
    /// </summary>
    /// <param name="registerUrl">The URL to register with the server.</param>
    /// <param name="receiverUrl">The URL to receive messages from the server.</param>
    /// <param name="ip">The IP address that will be listening.</param>
    /// <param name="port">The port that will be listening.</param>
    public Receiver(string registerUrl, string receiverUrl, IPAddress ip, int port)
    {
        _ip = ip;
        _port = port;
        _httpClient = new HttpClient();
        _registerUrl = registerUrl;
        _receiveUrl = receiverUrl;
    }

    /// <summary>
    /// Registers with the server, waits for a notification, and then disposes of the socket.
    /// </summary>
    public async Task WaitNotification()
    {
        Register().Wait();
        var socket = CreateSocket();
        var tcpClient = await socket.AcceptAsync();
        tcpClient.Dispose();
        socket.Dispose();
    }

    /// <summary>
    /// Sends a GET request to the server to receive a message, beautifies the message, and then displays it.
    /// </summary>
    public async Task ShowMessage()
    {
        var response = await _httpClient.GetAsync(_receiveUrl);
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadAsStringAsync();
        Console.WriteLine(Beautified(data));
    }

    /// <summary>
    /// Creates a socket and binds it to the specified IP and port.
    /// </summary>
    /// <returns>The created socket.</returns>
    private Socket CreateSocket()
    {
        var ipPoint = new IPEndPoint(_ip, _port);
        var tcpListener = new Socket(ipPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); 
        tcpListener.Bind(ipPoint);
        tcpListener.Listen();
        
        return tcpListener;
    }

    /// <summary>
    /// Sends a GET request to the server to register, parses the response, and stores the IP and port.
    /// </summary>
    private async Task Register()
    {
        var response = await _httpClient.GetAsync(_registerUrl);
        response.EnsureSuccessStatusCode();
        var pair = (await response.Content.ReadAsStringAsync()).Split(":");
    }

    /// <summary>
    /// Parses the JSON data, extracts the last message, and beautifies it.
    /// </summary>
    /// <param name="json">The JSON data to beautify.</param>
    /// <returns>The beautified message.</returns>
    private string Beautified(string json)
    {
        var messages = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

        if (messages is null)
        {
            Console.WriteLine("Fetched data is invalid");
            Environment.Exit(1);
        }
        
        var message = messages.Last();
        var id = message["id"];
        var content = message["content"];
        var date = message["date"]
           .Split(".")
           .SkipLast(1)
           .First()
           .Replace("T", " ");
        
        return $"{id}. Message: {content} | Sent at: {date}";
    }
}