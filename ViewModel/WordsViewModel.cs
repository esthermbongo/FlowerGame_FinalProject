using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlowerGame.Model;
using FlowerGame.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerGame.ViewModel
{
    public partial class WordsViewModel : BaseViewModel
    {
        private readonly WordService wordService;
        private readonly IConnectivity connectivity;


        public ObservableCollection<string> AvailableLanguages { get; } = new() { "English", "Spanish", "French" };
        public ObservableCollection<string> AvailableCategories { get; } = new() { "animal", "food", "plant", "person" };
        public ObservableCollection<string> AvailableLevels { get; } = new() { "Easy", "Medium", "Hard" };

        [ObservableProperty]
        string gameStatus = "Select language, level, and category to start";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanStartGame))]
        string selectedLanguage;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanStartGame))]
        string selectedCategory;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanStartGame))]
        string selectedLevel;

        public bool CanStartGame =>
            !string.IsNullOrWhiteSpace(SelectedLanguage) &&
            !string.IsNullOrWhiteSpace(SelectedCategory) &&
            !string.IsNullOrWhiteSpace(SelectedLevel);

        

        public WordsViewModel(WordService wordService, IConnectivity connectivity)
        {
            this.wordService = wordService;
            this.connectivity = connectivity;
        }

        [RelayCommand]
        public async Task StartGameAsync()
        {
            if (!CanStartGame) return;
            var navigationParameter = new Dictionary<string, object>
            {
                { "language", SelectedLanguage },
                { "level", SelectedLevel },
                { "category", SelectedCategory }
            };
            try
            {
                await Shell.Current.GoToAsync("///GamePage", navigationParameter);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "Navigation failed.", "OK");
            }
        }
    }
}
