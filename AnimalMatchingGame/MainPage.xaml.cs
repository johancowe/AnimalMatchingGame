using System;
using Microsoft.Maui.Controls;

namespace AnimalMatchingGame
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
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


        private void Button_Clicked(object sender, EventArgs e)
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
                AnimalButtons.IsVisible = false;
                PlayAgainButton.IsVisible = true;
                matchesFound = 0;
            }


        }
    }
}
