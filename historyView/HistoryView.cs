using Newtonsoft.Json;

namespace api.HistoryView;

public class HistoryView(string Url)
{
    private HttpClient _client = new HttpClient();

    public void ShowHistory()
    {
        ShowBeautified(FetchData().Result);
    }
    
    private async Task<string> FetchData()
    {
        using HttpResponseMessage response = await _client.GetAsync(Url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private void ShowBeautified(string json)
    {
        var messages = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

        if (messages is null)
        {
            Console.WriteLine("No messages");
            Environment.Exit(0);
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
            
            Console.WriteLine($"{id}. Message: {content} | Sent at: {date}");
        }
    }
}