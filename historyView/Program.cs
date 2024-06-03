namespace HistoryView;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.Write("Enter timestamp: ");
        var timestamp = int.Parse(Console.ReadLine()!);
        new HistoryView("http://localhost:5228/api/message/last-minutes").ShowHistory(timestamp);
    }
}