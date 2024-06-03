namespace api.HistoryView;

public static class Program
{
    public static void Main(string[] args)
    {
        new HistoryView("http://localhost:5228/api/message/last-10-minutes").ShowHistory();
    }
}