using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> _colors = new List<string> { "Red", "Yellow", "Orange", "White", "Green", "Blue" };
        private List<string> _code;
        private int _attemptsLeft = 10;
        private int _score = 100;
        private List<string> _players = new List<string>();
        private int _currentPlayerIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
            _code = new List<string>();
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
            _players.Clear(); // Clear old data
            _currentPlayerIndex = 0;

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
            MessageBox.Show($"Players ready! First player: {_players[_currentPlayerIndex]}.");
            ResetGameForCurrentPlayer();
        }
        private void ResetGameForCurrentPlayer()
        {
            // Randomize the code
            Random rand = new Random();
            _code = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                _code.Add(_colors[rand.Next(_colors.Count)]);
            }

            _attemptsLeft = 10;
            _score = 100;

            // Clear combo boxes
            ComboBox1.SelectedItem = null;
            ComboBox2.SelectedItem = null;
            ComboBox3.SelectedItem = null;
            ComboBox4.SelectedItem = null;

            // Reset UI 
            ScoreLabel.Content = $"Score: {_score}";
            AttemptsLabel.Content = $"Attempts Left: {_attemptsLeft}";

            
            CurrentPlayerLabel.Content = $"Current Player: {_players[_currentPlayerIndex]}";

            ListBoxHistory.Items.Clear();

            
            string currentPlayer = _players[_currentPlayerIndex];
            string nextPlayer = (_currentPlayerIndex + 1 < _players.Count) ? _players[_currentPlayerIndex + 1] : _players[0];

            MessageBox.Show($"New game started for {currentPlayer}! Try to guess the code.\nNext player: {nextPlayer}");
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
                // Update feedback 
                if (selectedColors[i] == _code[i])
                {
                    feedback.Add("Correct");
                    UpdateBorderColor(i, Brushes.DarkRed); // Correct color
                }
                else if (_code.Contains(selectedColors[i]))
                {
                    feedback.Add("Wrong Place");
                    UpdateBorderColor(i, Brushes.Wheat); // Incorrect position
                    _score -= 1;
                }
                else
                {
                    feedback.Add("Wrong");
                    UpdateBorderColor(i, Brushes.Transparent); // Incorrect color
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
                ProceedToNextPlayer();
            }
            else if (_attemptsLeft == 0)
            {
                MessageBox.Show($"Game over! The code was: {string.Join(", ", _code)}");
                ProceedToNextPlayer();
            }
        }
        private void HintButton_Click(object sender, RoutedEventArgs e)
        {
            if (_score < 15)  // Check if the player has enough points to buy a hint
            {
                MessageBox.Show("You don't have enough points to buy a hint.");
                return;
            }

            // Ask the player to choose a hint type using a message box
            MessageBoxResult result = MessageBox.Show("Do you want a hint for a correct color (15 points) or a correct color in the correct position (25 points)?",
                                                       "Buy a Hint", MessageBoxButton.YesNoCancel);

            // Handle the player's choice
            if (result == MessageBoxResult.Yes)  // Correct color hint
            {
                if (_score >= 15)
                {
                    _score -= 15;  // Deduct 15 points for a correct color hint
                    ProvideColorHint();
                }
                else
                {
                    MessageBox.Show("Not enough points to purchase this hint.");
                }
            }
            else if (result == MessageBoxResult.No)  // Correct color in the correct position hint
            {
                if (_score >= 25)
                {
                    _score -= 25;  // Deduct 25 points for a correct color in correct position hint
                    ProvidePositionHint();
                }
                else
                {
                    MessageBox.Show("Not enough points to purchase this hint.");
                }
            }
            else  // Cancelled
            {
                MessageBox.Show("Hint purchase cancelled.");
            }

            // Update the UI after the hint
            ScoreLabel.Content = $"Score: {_score}";
        }
        private void ProvideColorHint()
        {
            // Find the correct colors
            List<string> selectedColors = new List<string>
            {
                ComboBox1.SelectedItem?.ToString() ?? "unknown",
                ComboBox2.SelectedItem?.ToString() ?? "unknown",
                ComboBox3.SelectedItem?.ToString() ?? "unknown",
                ComboBox4.SelectedItem?.ToString() ?? "unknown"
            };

            List<string> correctColors = selectedColors.Where(color => _code.Contains(color)).ToList();

            if (correctColors.Any())
            {
                MessageBox.Show($"Hint: Correct color(s) in your guess: {string.Join(", ", correctColors)}");
            }
            else
            {
                MessageBox.Show("No correct colors in your guess.");
            }
        }
        private void ProvidePositionHint()
        {
            // Check for colors that are correct and in the correct position
            List<string> selectedColors = new List<string>
            {
                ComboBox1.SelectedItem?.ToString() ?? "unknown",
                ComboBox2.SelectedItem?.ToString() ?? "unknown",
                ComboBox3.SelectedItem?.ToString() ?? "unknown",
                ComboBox4.SelectedItem?.ToString() ?? "unknown"
            };

            List<string> correctPositions = new List<string>();

            for (int i = 0; i < selectedColors.Count; i++)
            {
                if (selectedColors[i] == _code[i])
                {
                    correctPositions.Add($"{selectedColors[i]} (Position {i + 1})");
                }
            }

            if (correctPositions.Any())
            {
                MessageBox.Show($"Hint: Correct color(s) in the correct position: {string.Join(", ", correctPositions)}");
            }
            else
            {
                MessageBox.Show("No colors are correct and in the correct position.");
            }
        }

        private void ProceedToNextPlayer()
        {
            
            _currentPlayerIndex++;

            if (_currentPlayerIndex >= _players.Count)
            {
                _currentPlayerIndex = 0; 
            }

         
            ResetGameForCurrentPlayer();
        }

        private void UpdateBorderColor(int index, Brush color)
        {
            string tooltipText = "";

            // Apply border color and set the tooltip based on feedback
            switch (index)
            {
                case 0:
                    Border1.BorderBrush = color;
                    tooltipText = GetTooltipText(color);
                    Border1.ToolTip = tooltipText;  //  tooltip for Border1
                    break;
                case 1:
                    Border2.BorderBrush = color;
                    tooltipText = GetTooltipText(color);
                    Border2.ToolTip = tooltipText;  //  tooltip for Border2
                    break;
                case 2:
                    Border3.BorderBrush = color;
                    tooltipText = GetTooltipText(color);
                    Border3.ToolTip = tooltipText;  //  tooltip for Border3
                    break;
                case 3:
                    Border4.BorderBrush = color;
                    tooltipText = GetTooltipText(color);
                    Border4.ToolTip = tooltipText;  //  tooltip for Border4
                    break;
            }
        }
        private string GetTooltipText(Brush color)
        {
            // Determine the tooltip text based on the border color
            if (color == Brushes.Wheat)
            {
                return "Juiste kleur, foute positie";  // Correct color, wrong position
            }
            else if (color == Brushes.DarkRed)
            {
                return "Juiste kleur, juiste positie";  // Correct color, correct position
            }
            else if (color == Brushes.Transparent)
            {
                return "Foute kleur";  // Wrong color
            }
            else
            {
                return "";  // No feedback or color
            }
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
    }
}