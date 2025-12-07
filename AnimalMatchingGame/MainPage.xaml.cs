using System;
using System.Text.Json;
using Microsoft.Maui.Controls;

namespace AnimalMatchingGame
{
    public partial class MainPage : ContentPage
    {
        private List<LeaderboardEntry> leaderboard = [];

        private static string LeaderboardFilePath =>
            Path.Combine(FileSystem.AppDataDirectory, "leaderboard.json");

        public MainPage()
        {
            InitializeComponent();
            LoadLeaderboard();
            UpdateLeaderboardDisplay();
        }

        private void LoadLeaderboard()
        {
            // Toon het pad in Debug output (View > Output > Debug)
            System.Diagnostics.Debug.WriteLine($"📁 Leaderboard bestand: {LeaderboardFilePath}");

            if (File.Exists(LeaderboardFilePath))
            {
                var json = File.ReadAllText(LeaderboardFilePath);
                leaderboard = JsonSerializer.Deserialize<List<LeaderboardEntry>>(json) ?? [];
            }
        }

        private void SaveLeaderboard()
        {
            var json = JsonSerializer.Serialize(leaderboard, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(LeaderboardFilePath, json);
        }

        private bool IsTopThreeTime(double timeInSeconds)
        {
            if (leaderboard.Count < 3)
                return true;

            return timeInSeconds < leaderboard.Max(e => e.TimeInSeconds);
        }

        private async Task AddScoreToLeaderboard(double timeInSeconds)
        {
            string playerName = "Anoniem";

            // Vraag om naam als het een top 3 tijd is
            if (IsTopThreeTime(timeInSeconds))
            {
                var name = await DisplayPromptAsync(
                    "🏆 Top 3 Tijd!",
                    $"Je hebt een top 3 tijd behaald: {timeInSeconds:0.0}s\nWat is je naam?",
                    "OK",
                    "Annuleren",
                    "Je naam",
                    maxLength: 20,
                    keyboard: Keyboard.Text);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    playerName = name;
                }
            }

            var entry = new LeaderboardEntry
            {
                PlayerName = playerName,
                TimeInSeconds = timeInSeconds,
                AchievedAt = DateTime.Now
            };

            leaderboard.Add(entry);
            leaderboard = [.. leaderboard.OrderBy(e => e.TimeInSeconds).Take(3)];
            SaveLeaderboard();
            UpdateLeaderboardDisplay();
        }

        private void UpdateLeaderboardDisplay()
        {
            Score1Label.Text = leaderboard.Count > 0
                ? $"1. {leaderboard[0].PlayerName} - {leaderboard[0].FormattedTime} ({leaderboard[0].FormattedDate})"
                : "1. ---";

            Score2Label.Text = leaderboard.Count > 1
                ? $"2. {leaderboard[1].PlayerName} - {leaderboard[1].FormattedTime} ({leaderboard[1].FormattedDate})"
                : "2. ---";

            Score3Label.Text = leaderboard.Count > 2
                ? $"3. {leaderboard[2].PlayerName} - {leaderboard[2].FormattedTime} ({leaderboard[2].FormattedDate})"
                : "3. ---";
        }

        private void PlayAgainButton_Clicked(object sender, EventArgs e)
        {
            // TODO: Add logic to reset the game or start a new game
            AnimalButtons.IsVisible = true;
            PlayAgainButton.IsVisible = false;

            List<String> animalEmoji = [
                "🐙","🐙",
                "🐁","🐁",
                "🐄","🐄",
                "🐈‍","🐈‍",
                "🐍","🐍",
                "🦆","🦆",
                "🐧","🐧",
                "🐟","🐟",
             ];

            foreach (var button in AnimalButtons.Children.OfType<Button>())
            {
                int index = Random.Shared.Next(animalEmoji.Count);
                string nextEmoji = animalEmoji[index];
                button.Text = nextEmoji;
                button.BackgroundColor = Colors.LightBlue;
                animalEmoji.RemoveAt(index);
            }

            Dispatcher.StartTimer(TimeSpan.FromSeconds(.1), TimerTick);
        }

        int tenthsOfSecondsElapsed = 0;

        private bool TimerTick()
        {
            if (!this.IsLoaded) return false;

            tenthsOfSecondsElapsed++;

            TimeElapsed.Text = "Time elapsed: " + (tenthsOfSecondsElapsed / 10F).ToString("0.0s");

            if (PlayAgainButton.IsVisible)
            {
                tenthsOfSecondsElapsed = 0;
                return false;
            }

            return true;

        }



        Button lastClicked;
        bool findingMatch = false;
        int matchesFound;


        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (sender is Button buttonClicked)
            {

                if (!string.IsNullOrEmpty(buttonClicked.Text) && (findingMatch == false))
                {
                    buttonClicked.BackgroundColor = Colors.Red;
                    lastClicked = buttonClicked;
                    findingMatch = true;
                }
                else
                {
                    if ((buttonClicked != lastClicked) && (buttonClicked.Text == lastClicked.Text) && (!String.IsNullOrWhiteSpace(buttonClicked.Text)))
                    {
                        matchesFound++;
                        lastClicked.Text = " ";
                        buttonClicked.Text = " ";
                    }
                    lastClicked.BackgroundColor = Colors.LightBlue;
                    buttonClicked.BackgroundColor = Colors.LightBlue;
                    findingMatch = false;
                }
            }

            if (matchesFound == 8)
            {
                // Voeg score toe aan leaderboard
                double finalTime = tenthsOfSecondsElapsed / 10.0;
                await AddScoreToLeaderboard(finalTime);

                AnimalButtons.IsVisible = false;
                PlayAgainButton.IsVisible = true;
                matchesFound = 0;
            }
        }
    }
}
