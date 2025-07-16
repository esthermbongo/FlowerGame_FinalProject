using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlowerGame.Model;
using FlowerGame.Services;
using FlowerGame.Views;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;

namespace FlowerGame.ViewModel
{
    public partial class GameViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly GameService gameService;
        private readonly WordService wordService;
        private GamePage? gamePage;

        public int RemainingLives => GameState?.MaxWrongGuesses - GameState?.WrongGuesses ?? 6;

        [ObservableProperty]
        GameState gameState = new();

        [ObservableProperty]
        string currentGuess = string.Empty;

        [ObservableProperty]
        string language = string.Empty;

        [ObservableProperty]
        string level = string.Empty;

        [ObservableProperty]
        string category = string.Empty;

        private ObservableCollection<string> letters = new();
        public ObservableCollection<string> Letters => letters;

        [ObservableProperty]
        bool hintUsed = false;

        public string WrongLettersDisplay => GameState?.WrongLetters is { Count: > 0 }
            ? string.Join(", ", GameState.WrongLetters)
            : "";

        public GameViewModel(GameService gameService, WordService wordService)
        {
            this.gameService = gameService;
            this.wordService = wordService;
        }

        public void SetGamePageReference(GamePage page) => gamePage = page;

        private void UpdateLettersForLanguage(string language)
        {
            letters.Clear();
            var basicLetters = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            foreach (var letter in basicLetters)
                letters.Add(letter);

            var accentedLetters = language?.ToLowerInvariant() switch
            {
                "spanish" => new[] { "Á", "É", "Í", "Ó", "Ú", "Ñ" },
                "french" => new[] { "À", "Â", "Ç", "È", "Ê", "Ë", "Î", "Ï", "Ô", "Ù", "Û", "Ü", "Ÿ" },
                _ => Array.Empty<string>()
            };
            foreach (var letter in accentedLetters)
                letters.Add(letter);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("language", out var lang))
                Language = lang?.ToString() ?? "";
            if (query.TryGetValue("level", out var lev))
                Level = lev?.ToString() ?? "";
            if (query.TryGetValue("category", out var cat))
                Category = cat?.ToString() ?? "";
        }

        [RelayCommand]
        public async Task InitializeGameAsync()
        {
            if (string.IsNullOrEmpty(Language) || string.IsNullOrEmpty(Level) || string.IsNullOrEmpty(Category))
                return;

            try
            {
                IsBusy = true;
                HintUsed = false;
                UpdateLettersForLanguage(Language);
                var words = await wordService.GetWords(Language, Category, Level).ConfigureAwait(false);
                if (words?.Count > 0)
                {
                    var randomWord = words[Random.Shared.Next(words.Count)];
                    GameState = gameService.InitializeGame(randomWord.WordText, Language);
                    OnPropertyChanged(nameof(WrongLettersDisplay));
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to start game: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AnimateCorrectGuessAsync()
        {
            if (gamePage == null) return;
            var flowerImage = gamePage.FindByName<Image>("MainFlowerImage");
            if (flowerImage == null) return;

            await Task.WhenAll(
                flowerImage.ScaleTo(1.15, 150, Easing.BounceOut),
                flowerImage.RotateTo(5, 100)
            );
            await Task.WhenAll(
                flowerImage.ScaleTo(1.0, 150, Easing.BounceOut),
                flowerImage.RotateTo(-5, 100)
            );
            await flowerImage.RotateTo(0, 150, Easing.BounceOut);
            await CreateSparkleAnimation();
        }

        private async Task CreateSparkleAnimation()
        {
            if (gamePage == null) return;
            var container = gamePage.FindByName<Grid>("PetalContainer");
            if (container == null) return;

            for (int i = 0; i < 3; i++)
            {
                var sparkle = new Label
                {
                    Text = "✨",
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                sparkle.TranslationX = Random.Shared.Next(-60, 60);
                sparkle.TranslationY = Random.Shared.Next(-60, 60);
                container.Children.Add(sparkle);

                _ = Task.Run(async () =>
                {
                    await Task.WhenAll(
                        sparkle.ScaleTo(1.5, 300, Easing.BounceOut),
                        sparkle.FadeTo(0, 800, Easing.CubicOut)
                    );
                    Device.BeginInvokeOnMainThread(() => container.Children.Remove(sparkle));
                });

                await Task.Delay(100);
            }
        }

        [RelayCommand]
        public async Task AnimateWrongGuessAsync()
        {
            if (gamePage == null) return;
            var flowerImage = gamePage.FindByName<Image>("MainFlowerImage");
            if (flowerImage == null) return;

            await Task.WhenAll(
                flowerImage.ScaleTo(0.9, 150),
                flowerImage.RotateTo(-10, 100)
            );
            await flowerImage.RotateTo(10, 100);
            await flowerImage.RotateTo(0, 100, Easing.BounceOut);
            await flowerImage.ScaleTo(1.0, 150, Easing.BounceOut);
            await CreateFallingPetalAnimation(GameState.WrongGuesses);
        }

        private async Task CreateFallingPetalAnimation(int petalIndex)
        {
            if (gamePage == null) return;
            var container = gamePage.FindByName<Grid>("PetalContainer");
            if (container == null) return;

            var fallingPetal = new Label
            {
                Text = "🌸",
                FontSize = 30,
                TextColor = Color.FromArgb("#FFB6C1"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            fallingPetal.TranslationX = Random.Shared.Next(-40, 40);
            fallingPetal.TranslationY = Random.Shared.Next(-40, 40);
            container.Children.Add(fallingPetal);

            await Task.WhenAll(
                fallingPetal.TranslateTo(
                    Random.Shared.Next(-100, 100),
                    300,
                    2500,
                    Easing.BounceOut),
                fallingPetal.RotateTo(Random.Shared.Next(-360, 360), 2500),
                fallingPetal.FadeTo(0, 2500, Easing.CubicOut),
                fallingPetal.ScaleTo(0.3, 2500, Easing.CubicIn)
            );
            container.Children.Remove(fallingPetal);
        }

        [RelayCommand]
        public async Task GuessLetterAsync(string letter)
        {
            if (string.IsNullOrEmpty(letter) || GameState.IsGameOver)
                return;

            var wasCorrect = GameState.CurrentWord.Contains(letter.ToUpperInvariant()[0]);
            GameState = gameService.ProcessGuess(GameState, letter[0]);

            if (wasCorrect)
                await AnimateCorrectGuessAsync();
            else
                await AnimateWrongGuessAsync();

            OnPropertyChanged(nameof(GameState));
            OnPropertyChanged(nameof(WrongLettersDisplay));
            OnPropertyChanged(nameof(RemainingLives));
            CurrentGuess = string.Empty;

            if (GameState.IsGameOver)
            {
                var message = GameState.IsWinner
                    ? "Congratulations! You won! 🌸"
                    : $"Game Over! The word was: {GameState.CurrentWord} 🥀";
                await Shell.Current.DisplayAlert("Game Over", message, "OK");
            }
        }

        [RelayCommand]
        public async Task NewGameAsync()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        [RelayCommand]
        public async Task ShowHintAsync()
        {
            if (HintUsed)
            {
                await Shell.Current.DisplayAlert("Hint", "You've already used your hint for this word!", "OK");
                return;
            }

            if (string.IsNullOrEmpty(GameState?.CurrentWord))
            {
                await Shell.Current.DisplayAlert("Hint", "No word to get hint for!", "OK");
                return;
            }

            
            var hint = CreateCustomHint(GameState.CurrentWord, Category, Language);

            await Shell.Current.DisplayAlert("💡 Hint", hint, "Got it!");
            HintUsed = true;
        }

        private string CreateCustomHint(string word, string category, string language)
        {
            var hints = new Dictionary<string, List<string>>
            {
                ["animal"] = new() {
            "This creature lives in the wild or as a pet",
            "You might see this at a zoo or farm",
            "This animal has fur, feathers, or scales",
            "This creature might swim, fly, or walk",
            "You might hear this animal make sounds"
        },
                ["food"] = new() {
            "You can eat this!",
            "This might be found in a kitchen or restaurant",
            "This could be sweet, salty, or spicy",
            "You might cook this or eat it raw",
            "This could be a main dish, snack, or dessert"
        },
                ["plant"] = new() {
            "This grows in the ground or in a pot",
            "This is green and needs water and sunlight",
            "You might find this in a garden or forest",
            "This could have flowers, leaves, or fruit",
            "Bees and butterflies might visit this"
        },
                ["person"] = new() {
            "This describes a type of person",
            "This is someone you might know or meet",
            "This person has a special role or job",
            "This could be someone in your family or community",
            "This person might help others or have special skills"
        }
            };

            var categoryHints = hints.ContainsKey(category.ToLower()) ?
                hints[category.ToLower()] : new() { "This word fits the category you selected" };

            var randomHint = categoryHints[Random.Shared.Next(categoryHints.Count)];

            return $"💡 {randomHint}\n\n🔤 The word has {word.Length} letters\n🌍 Language: {language}\n🎯 Category: {category}";
        }
    }
}