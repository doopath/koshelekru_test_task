using System.Net.Http.Json;

namespace Sender;

public static class Program
{
    public static async Task Main(string[] args)
    {
        while (true)
        {
            var httpClient = GetHttpClient();
            var message = GetMessage();
            var response = await SendRequest(httpClient, message);
            response.EnsureSuccessStatusCode();
            ShowSuccessMessage();
        }
    }

    private static string GetMessage()
    {
        Console.Write("Enter message: ");
        var message = Console.ReadLine()!;

        if (message.Length <= 128) return message;
        
        Console.WriteLine("Message must be less than 128 characters long");
        Environment.Exit(1);
        return "";
    }

    private static HttpClient GetHttpClient()
        => new()
        {
            BaseAddress = new Uri("http://localhost:5228/api/message/"),
        };
    
    private static void ShowSuccessMessage()
    {
        Console.WriteLine("Message sent successfully\n");
    }

    private static async Task<HttpResponseMessage> SendRequest(HttpClient httpClient, string message)
    {
        try
        {
            return await httpClient.PostAsJsonAsync(
                "send",
                new MessageDTO() { Content = message });
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Connection refused! Are you sure you started up the api service?");
            Environment.Exit(1);
            return null;
        }
    }
}