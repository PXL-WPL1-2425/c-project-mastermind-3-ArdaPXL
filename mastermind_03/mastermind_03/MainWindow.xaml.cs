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
    public partial class MainWindow : Window
    {
        private List<string> _colors = new List<string> { "Red", "Yellow", "Orange", "White", "Green", "Blue" };
        private List<string> _availableColors;
        private List<string> _code;
        private int _attemptsLeft = 10;
        private int _score = 100;
        private List<string> _players = new List<string>();
        private int _currentPlayerIndex = 0;
        private int _selectedColorCount = 4; // Default 4 colors

        public MainWindow()
        {
            InitializeComponent();
            _availableColors = new List<string> { "Red", "Yellow", "Orange", "White", "Green", "Blue" }; // Default color options
            StartNewGame();
            PopulateComboBoxes();
        }

        private void ColorCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorCountComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedContent = selectedItem.Content.ToString();

                
                switch (selectedContent)
                {
                    case "4 Colors":
                        _selectedColorCount = 4;
                        break;
                    case "5 Colors":
                        _selectedColorCount = 5;
                        break;
                    case "6 Colors":
                        _selectedColorCount = 6;
                        break;
                }

                
                UpdateAvailableColors();
            }
        }

        private void UpdateAvailableColors()
        {
            
            if (_selectedColorCount == 4)
            {
                _availableColors = new List<string> { "Red", "Yellow", "Orange", "White" };
            }
            else if (_selectedColorCount == 5)
            {
                _availableColors = new List<string> { "Red", "Yellow", "Orange", "White", "Green" };
            }
            else if (_selectedColorCount == 6)
            {
                _availableColors = new List<string> { "Red", "Yellow", "Orange", "White", "Green", "Blue" };
            }

         
            PopulateComboBoxes();
        }

        private void PopulateComboBoxes()
        {
           
            ComboBox1.ItemsSource = _availableColors;
            ComboBox2.ItemsSource = _availableColors;
            ComboBox3.ItemsSource = _availableColors;
            ComboBox4.ItemsSource = _availableColors;
        }

        private void StartNewGame()
        {
            _players.Clear(); // Clear old  data
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
            Random rand = new Random();
            _code = new List<string>();

            // Generate a random code 
            for (int i = 0; i < 4; i++)  // The code always has 4 positions
            {
                _code.Add(_availableColors[rand.Next(_selectedColorCount)]);  // Randomly select from the available colors
            }

            _attemptsLeft = 10;
            _score = 100;

            ComboBox1.SelectedItem = null;
            ComboBox2.SelectedItem = null;
            ComboBox3.SelectedItem = null;
            ComboBox4.SelectedItem = null;

            ScoreLabel.Content = $"Score: {_score}";
            AttemptsLabel.Content = $"Attempts Left: {_attemptsLeft}";
            PlayerLabel.Content = $"Current Player: {_players[_currentPlayerIndex]}";
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
                // Update feedback based on correctness
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
                AskToPlayAgain();
            }
            else if (_attemptsLeft == 0)
            {
                MessageBox.Show($"Game over! The code was: {string.Join(", ", _code)}");
                AskToPlayAgain();
            }
        }

        private void AskToPlayAgain()
        {
            if (MessageBox.Show("Do you want to play again?", "Game Over", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                StartNewGame();
            }
            else
            {
                Close();
            }
        }

        private void UpdateBorderColor(int index, Brush color)
        {
            // Apply border color based on feedback
            switch (index)
            {
                case 0:
                    Border1.BorderBrush = color;
                    break;
                case 1:
                    Border2.BorderBrush = color;
                    break;
                case 2:
                    Border3.BorderBrush = color;
                    break;
                case 3:
                    Border4.BorderBrush = color;
                    break;
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

        private void BuyHintButton_Click(object sender, RoutedEventArgs e)
        {
            if (_score > 5)
            {
                _score -= 5;
                ScoreLabel.Content = $"Score: {_score}";

                Random rand = new Random();
                int hintIndex = rand.Next(4);
                MessageBox.Show($"Hint: Color {hintIndex + 1} is {_code[hintIndex]}.");
            }
            else
            {
                MessageBox.Show("Not enough score for a hint.");
            }
        }
    }
}