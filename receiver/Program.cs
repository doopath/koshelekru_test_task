namespace Receiver;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var registerUrl = "http://localhost:5228/api/message/in-real-time/8080";
        var receiveUrl = "http://localhost:5228/api/message/last-minutes/1";

        while (true)
        {
            var receiver = new Receiver(registerUrl, receiveUrl);
            await receiver.WaitNotification();
            await receiver.ShowMessage();
        }
    }
}