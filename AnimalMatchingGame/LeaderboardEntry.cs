namespace AnimalMatchingGame;

public class LeaderboardEntry
{
    public string PlayerName { get; set; } = "";
    public double TimeInSeconds { get; set; }
    public DateTime AchievedAt { get; set; }

    public string FormattedTime => TimeInSeconds.ToString("0.0") + "s";
    public string FormattedDate => AchievedAt.ToString("dd MMM HH:mm");
}
