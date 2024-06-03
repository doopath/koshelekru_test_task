using System.Net.Http.Json;

namespace Sender;

/// <summary>
/// Contains the main entry point for the console application.
/// </summary>
public static class Program
{
    /// <summary>
    /// The main entry point for the console application.
    /// Sends messages to a specified API endpoint continuously.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
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

    /// <summary>
    /// Gets a message from the user input.
    /// </summary>
    /// <returns>The user's input message.</returns>
    private static string GetMessage()
    {
        Console.Write("Enter message: ");
        var message = Console.ReadLine()!;

        if (message.Length <= 128) return message;
        
        Console.WriteLine("Message must be less than 128 characters long");
        Environment.Exit(1);
        return "";
    }

    /// <summary>
    /// Creates a new HttpClient instance with the specified base address.
    /// </summary>
    /// <returns>A new HttpClient instance.</returns>
    private static HttpClient GetHttpClient()
        => new()
        {
            BaseAddress = new Uri("http://localhost:5228/api/message/"),
        };
    
    /// <summary>
    /// Displays a success message to the console.
    /// </summary>
    private static void ShowSuccessMessage()
    {
        Console.WriteLine("Message sent successfully\n");
    }

    /// <summary>
    /// Sends a POST request to the specified API endpoint with the given message.
    /// </summary>
    /// <param name="httpClient">The HttpClient instance to use for the request.</param>
    /// <param name="message">The message to send.</param>
    /// <returns>The HttpResponseMessage from the API.</returns>
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