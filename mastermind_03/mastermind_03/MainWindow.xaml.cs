
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;

namespace mastermind_03
{
    public partial class MainWindow : Window
    {
        private List<string> _colors = new List<string> { "Red", "Yellow", "Orange", "White", "Green", "Blue" };
        private List<string> _availableColors;
        private List<string> _code;
        private int _attemptsLeft = 10;
        private int _score = 100;
        private List<string> _players = new List<string>();
        private int _currentPlayerIndex = 0;
        private int _selectedColorCount = 4; // Default to 4 colors
        private string[] highscores = new string[15];
        private int highscoreCount = 0;
        private int _attempts = 0;
        private string _playerName;
        public MainWindow()
        {
            InitializeComponent();
            _availableColors = new List<string> { "Red", "Yellow", "Orange", "White", "Green", "Blue" }; // 
            StartNewGame();
            PopulateComboBoxes();
        }
        private void PopulateComboBoxes()
        {

            List<string> colorOptions = new List<string> { "Red", "Yellow", "Orange", "White", "Green", "Blue" };


            ComboBox1.ItemsSource = colorOptions;
            ComboBox2.ItemsSource = colorOptions;
            ComboBox3.ItemsSource = colorOptions;
            ComboBox4.ItemsSource = colorOptions;
        }



        private void StartNewGame()
        {
            _players.Clear(); // Clear data
            _currentPlayerIndex = 0;

            // Move this line after players are added
            // _playerName = _players[_currentPlayerIndex];

            do
            {
                string playerName = Microsoft.VisualBasic.Interaction.InputBox("Enter player name:", "Add Player");

                if (!string.IsNullOrWhiteSpace(playerName))
                {
                    _players.Add(playerName);
                }
                else
                {
                    MessageBox.Show("Player name cannot be empty.");
                }

            } while (MessageBox.Show("Add another player?", "Add Player", MessageBoxButton.YesNo) == MessageBoxResult.Yes);

            if (_players.Count == 0)
            {
                MessageBox.Show("At least one player is required to start the game.");
                return;
            }

            // Now that players have been added, assign the player name
            _playerName = _players[_currentPlayerIndex];

            MessageBox.Show($"Players ready! First player: {_players[_currentPlayerIndex]}.");
            UpdatePlayerLabels();
            ResetGameForCurrentPlayer();
        }
        private void Afsluiten_Click(object sender, RoutedEventArgs e)
        {

            Close();

        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
            StartNewGame();
        }
        private void ResetUI()
        {
            // Reset Ellipses
            FeedbackEllipse1.Fill = Brushes.Transparent;
            FeedbackEllipse2.Fill = Brushes.Transparent;
            FeedbackEllipse3.Fill = Brushes.Transparent;
            FeedbackEllipse4.Fill = Brushes.Transparent;
        }

        private void pogingen_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Aantal pogingen: {_attempts}");
        }





        private void UpdatePlayerLabels()
        {

            PlayersStackPanel.Children.Clear();


            foreach (string player in _players)
            {
                System.Windows.Controls.Label playerLabel = new System.Windows.Controls.Label
                {
                    Content = player,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 150,
                    Margin = new Thickness(10, 10 + 30 * _players.IndexOf(player), 0, 0),
                    Background = _players.IndexOf(player) == _currentPlayerIndex ? Brushes.LightGreen : Brushes.Transparent,
                    Padding = new Thickness(5)
                };

                PlayersStackPanel.Children.Add(playerLabel);
            }
        }


        private void RegistreerHighscore(string playerName, int score, int attempts)
        {
            if (highscoreCount < highscores.Length)
            {
                highscores[highscoreCount++] = $"{playerName} - Pogingen: {attempts} - Score: {score}";
            }
        }
        private void ShowHighscores()
        {
            if (highscoreCount == 0)
            {
                MessageBox.Show("Er zijn nog geen highscores.", "Mastermind highscores");
            }
            else
            {
                // Voeg niet-lege entries samen om ze in een lijst weer te geven
                string highscoreList = string.Join("\n", highscores.Where(h => !string.IsNullOrEmpty(h)));
                MessageBox.Show(highscoreList, "Mastermind highscores");
            }
        }
        private void Highscores_Click(object sender, RoutedEventArgs e)
        {
            ShowHighscores();
        }
        private void AddHighscore(string playerName, int attempts, int score)
        {
            if (highscoreCount < highscores.Length)
            {
                highscores[highscoreCount] = $"{playerName} - {attempts} pogingen - {score}/100";
                highscoreCount++;
            }
            else
            {
                MessageBox.Show("De highscorelijst is vol. Oude scores moeten worden overschreven.", "Highscores vol");
            }
        }





        private void ResetGameForCurrentPlayer()
        {
            Random rand = new Random();
            _code = new List<string>();


            for (int i = 0; i < _selectedColorCount; i++)
            {
                _code.Add(_availableColors[rand.Next(_availableColors.Count)]);
            }

            _attemptsLeft = 10;
            _score = 100;

            ComboBox1.SelectedItem = null;
            ComboBox2.SelectedItem = null;
            ComboBox3.SelectedItem = null;
            ComboBox4.SelectedItem = null;

            ScoreLabel.Content = $"Score: {_score}";
            AttemptsLabel.Content = $"Attempts Left: {_attemptsLeft}";

            ListBoxHistory.Items.Clear();

            MessageBox.Show($"New game started for {_players[_currentPlayerIndex]}! Try to guess the code.");
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedColors = new List<string>
            {
                ComboBox1.SelectedItem?.ToString() ?? "unknown",
                ComboBox2.SelectedItem?.ToString() ?? "unknown",
                ComboBox3.SelectedItem?.ToString() ?? "unknown",
                ComboBox4.SelectedItem?.ToString() ?? "unknown"
            };

            if (selectedColors.Contains("unknown"))
            {
                MessageBox.Show("Please select all colors before checking.");
                return;
            }

            List<string> feedback = new List<string>();
            for (int i = 0; i < selectedColors.Count; i++)
            {

                if (selectedColors[i] == _code[i])
                {
                    feedback.Add("Correct");
                }
                else if (_code.Contains(selectedColors[i]))
                {
                    feedback.Add("Wrong Place");
                    _score -= 1;
                }
                else
                {
                    feedback.Add("Wrong");
                    _score -= 2;
                }
            }
           


            ScoreLabel.Content = $"Score: {_score}";
            AttemptsLabel.Content = $"Attempts Left: {_attemptsLeft}";
            ListBoxHistory.Items.Add($"Attempt: {string.Join(", ", selectedColors)} | Feedback: {string.Join(", ", feedback)}");

            _attemptsLeft--;


            if (selectedColors.SequenceEqual(_code))
            {
                MessageBox.Show($"You guessed the code! Final Score: {_score}");
                AskToPlayAgain();
                AddHighscore(_playerName, _score, _attempts);
            }
            else if (_attemptsLeft == 0)
            {
                MessageBox.Show($"Game over! The code was: {string.Join(", ", _code)}");
                AskToPlayAgain();
                AddHighscore(_playerName, _score, _attempts);
            }
        }

        private void AskToPlayAgain()
        {
            if (MessageBox.Show("Do you want to play again?", "Game Over", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
                UpdatePlayerLabels();
                ResetGameForCurrentPlayer();
                RegistreerHighscore(_playerName, _score, _attempts);
            }
            else
            {
                Close();
            }
        }
        private void BuyHintButton_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult result = MessageBox.Show(
                "Do you want to buy a hint?\n" +
                "A correct color costs 15 points.\n" +
                "A correct color in the correct position costs 25 points.",
                "Buy a Hint", MessageBoxButton.YesNoCancel);

            if (result == MessageBoxResult.Yes)
            {

                MessageBoxResult hintChoice = MessageBox.Show(
                    "Choose a hint type:\n" +
                    "Yes - Correct color\n" +
                    "No - Correct color in correct position",
                    "Select Hint Type", MessageBoxButton.YesNo);

                if (hintChoice == MessageBoxResult.Yes)
                {

                    if (_score >= 15)
                    {
                        _score -= 15;

                        string hint = GetCorrectColorHint();
                        MessageBox.Show($"Hint: {hint} is in the code but not in the correct position.");
                    }
                    else
                    {
                        MessageBox.Show("Not enough points to buy this hint.");
                    }
                }
                else if (hintChoice == MessageBoxResult.No)
                {

                    if (_score >= 25)
                    {
                        _score -= 25;

                        string hint = GetCorrectColorInCorrectPositionHint();
                        MessageBox.Show($"Hint: {hint} is in the code and in the correct position.");
                    }
                    else
                    {
                        MessageBox.Show("Not enough points to buy this hint.");
                    }
                }
            }
        }
        private string GetCorrectColorHint()
        {

            List<string> availableColors = new List<string>(_code);
            List<string> guessedColors = new List<string>
    {
        ComboBox1.SelectedItem?.ToString() ?? "unknown",
        ComboBox2.SelectedItem?.ToString() ?? "unknown",
        ComboBox3.SelectedItem?.ToString() ?? "unknown",
        ComboBox4.SelectedItem?.ToString() ?? "unknown"
    };


            for (int i = 0; i < 4; i++)
            {
                if (guessedColors[i] == _code[i])
                {
                    availableColors.Remove(guessedColors[i]);
                    guessedColors[i] = "correct";
                }
            }


            for (int i = 0; i < 4; i++)
            {
                if (guessedColors[i] != "correct" && availableColors.Contains(guessedColors[i]))
                {
                    availableColors.Remove(guessedColors[i]);
                    return guessedColors[i];
                }
            }

            return "No color found";
        }

        


        private SolidColorBrush GetBrushFromColorName(string colorName)
        {
            switch (colorName)
            {
                case "Rood": return Brushes.Red;
                case "Geel": return Brushes.Yellow;
                case "Oranje": return Brushes.Orange;
                case "Wit": return Brushes.White;
                case "Groen": return Brushes.Green;
                case "Blauw": return Brushes.Blue;
                default: return Brushes.Transparent;
            }
        }


        private string GetCorrectColorInCorrectPositionHint()
        {

            List<string> guessedColors = new List<string>
        {
            ComboBox1.SelectedItem?.ToString() ?? "unknown",
            ComboBox2.SelectedItem?.ToString() ?? "unknown",
            ComboBox3.SelectedItem?.ToString() ?? "unknown",
            ComboBox4.SelectedItem?.ToString() ?? "unknown"
        };


            for (int i = 0; i < 4; i++)
            {
                if (guessedColors[i] == _code[i])
                {
                    return guessedColors[i];
                }
            }

            return "No color in correct position";
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                string selectedColor = comboBox.SelectedItem?.ToString() ?? "";
                Brush colorBrush;

                switch (selectedColor)
                {
                    case "Red":
                        colorBrush = Brushes.Red;
                        break;
                    case "Yellow":
                        colorBrush = Brushes.Yellow;
                        break;
                    case "Orange":
                        colorBrush = Brushes.Orange;
                        break;
                    case "White":
                        colorBrush = Brushes.White;
                        break;
                    case "Green":
                        colorBrush = Brushes.Green;
                        break;
                    case "Blue":
                        colorBrush = Brushes.Blue;
                        break;
                    default:
                        colorBrush = Brushes.Transparent;
                        break;
                }


                if (comboBox == ComboBox1) FeedbackEllipse1.Fill = colorBrush;
                if (comboBox == ComboBox2) FeedbackEllipse2.Fill = colorBrush;
                if (comboBox == ComboBox3) FeedbackEllipse3.Fill = colorBrush;
                if (comboBox == ComboBox4) FeedbackEllipse4.Fill = colorBrush;
            }
        }

        private void ColorCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string selectedColorCount = ColorCountComboBox.SelectedItem?.ToString() ?? "4";


            switch (selectedColorCount)
            {
                case "4":
                    _colors = new List<string> { "Red", "Yellow", "Orange", "White", "Green", "Blue" };
                    break;
                case "5":
                    _colors = new List<string> { "Red", "Yellow", "Orange", "White", "Green", "Blue", "Purple" };
                    break;
                case "6":
                    _colors = new List<string> { "Red", "Yellow", "Orange", "White", "Green", "Blue", "Purple", "Black" };
                    break;
                default:
                    _colors = new List<string> { "Red", "Yellow", "Orange", "White", "Green", "Blue" };
                    break;
            }


            ResetGameForCurrentPlayer();
        }

    }
}