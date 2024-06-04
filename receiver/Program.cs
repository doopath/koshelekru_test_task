using System.Net;

namespace Receiver;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var registerUrl = "http://localhost:5228/api/message/subscribe/8080";
        var receiveUrl = "http://localhost:5228/api/message/last-minutes/1";
        var ip = IPAddress.Parse("127.0.0.1");
        var port = 8080;

        while (true)
        {
            var receiver = new Receiver(registerUrl, receiveUrl, ip, port);
            await receiver.WaitNotification();
            await receiver.ShowMessage();
        }
    }
}