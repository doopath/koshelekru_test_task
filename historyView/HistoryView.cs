using Newtonsoft.Json;

namespace HistoryView;

/// <summary>
/// This class represents a view for displaying history data.
/// </summary>
public class HistoryView
{
    private readonly HttpClient _client;
    private readonly string _url;

    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryView"/> class.
    /// </summary>
    /// <param name="url">The base URL for fetching history data.</param>
    public HistoryView(string url)
    {
        _client = new HttpClient();
        _url = url;
    }

    /// <summary>
    /// Shows the history for a specific timestamp.
    /// </summary>
    /// <param name="timestamp">The timestamp for which to show the history.</param>
    public void ShowHistory(int timestamp)
    {
        ShowTimestamp(Beautified(FetchData(timestamp).Result));
    }

    /// <summary>
    /// Fetches data for a specific timestamp.
    /// </summary>
    /// <param name="timestamp">The timestamp for which to fetch data.</param>
    /// <returns>The fetched data as a JSON string.</returns>
    private async Task<string> FetchData(int timestamp)
    {
        using HttpResponseMessage response = await _client.GetAsync($"{_url}/{timestamp}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Beautifies the fetched data.
    /// </summary>
    /// <param name="json">The fetched data as a JSON string.</param>
    /// <returns>A list of beautified messages.</returns>
    private List<string> Beautified(string json)
    {
        var messages = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
        var beautified = new List<string>();

        if (messages is null)
        {
            Console.WriteLine("Fetched data is invalid");
            Environment.Exit(1);
        }

        foreach (var message in messages)
        {
            var id = message["id"];
            var content = message["content"];
            var date = message["date"]
               .Split(".")
               .SkipLast(1)
               .First()
               .Replace("T", " ");

            beautified.Add($"{id}. Message: {content} | Sent at: {date}");
        }

        return beautified;
    }

    /// <summary>
    /// Shows the beautified messages.
    /// </summary>
    /// <param name="beautified">The list of beautified messages.</param>
    private void ShowTimestamp(List<string> beautified)
    {
        foreach (var message in beautified)
            Console.WriteLine(message);
    }
}