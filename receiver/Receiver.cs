using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace Receiver;

public class Receiver
{
    private readonly HttpClient _httpClient;
    private readonly string _registerUrl;
    private readonly string _receiveUrl;
    private string _ip;
    private int _port;

    public Receiver(string registerUrl, string receiverUrl)
    {
        _ip = "";
        _port = 0;
        _httpClient = new();
        _registerUrl = registerUrl;
        _receiveUrl = receiverUrl;
    }

    public async Task WaitNotification()
    {
        Register().Wait();
        var socket = CreateSocket();
        var tcpClient = await socket.AcceptAsync();
        tcpClient.Dispose();
        socket.Dispose();
    }

    public async Task ShowMessage()
    {
        var response = await _httpClient.GetAsync(_receiveUrl);
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadAsStringAsync();
        Console.WriteLine(Beautified(data));
    }

    private Socket CreateSocket()
    {
        var ipPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
        var tcpListener = new Socket(ipPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); 
        tcpListener.Bind(ipPoint);
        tcpListener.Listen();
        
        return tcpListener;
    }

    private async Task Register()
    {
        var response = await _httpClient.GetAsync(_registerUrl);
        response.EnsureSuccessStatusCode();
        var pair = (await response.Content.ReadAsStringAsync()).Split(":");
        _port = int.Parse(pair.TakeLast(1).First());
        _ip = string.Join(":", pair.SkipLast(1));
    }

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