using Newtonsoft.Json;

namespace Sender.HistoryView;

public class HistoryView(string Url)
{
    private HttpClient _client = new HttpClient();

    public void ShowHistory(int timestamp)
    {
        ShowTimestamp(Beautified(FetchData(timestamp).Result));
    }
    
    private async Task<string> FetchData(int timestamp)
    {
        using HttpResponseMessage response = await _client.GetAsync($"{Url}/{timestamp}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

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

    private void ShowTimestamp(List<string> beautified)
    {
        foreach (var message in beautified)
            Console.WriteLine(message);
    }
}