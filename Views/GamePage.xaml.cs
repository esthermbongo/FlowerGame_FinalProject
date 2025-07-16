using FlowerGame.ViewModel;

namespace FlowerGame.Views
{
    public partial class GamePage : ContentPage
    {
        private readonly GameViewModel viewModel;

        public GamePage(GameViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Set the page reference for animations
            if (BindingContext is GameViewModel vm)
            {
                vm.SetGamePageReference(this);
                await Task.Delay(100);
                await vm.InitializeGameAsync();
            }

            System.Diagnostics.Debug.WriteLine("=== GAME PAGE APPEARING ===");

            // Wait a moment for ApplyQueryAttributes to be called
            await Task.Delay(100);

            System.Diagnostics.Debug.WriteLine($"Language: {viewModel.Language}");
            System.Diagnostics.Debug.WriteLine($"Level: {viewModel.Level}");
            System.Diagnostics.Debug.WriteLine($"Category: {viewModel.Category}");

            await viewModel.InitializeGameAsync();

            System.Diagnostics.Debug.WriteLine($"After init - DisplayWord: {viewModel.GameState.DisplayWord}");
            System.Diagnostics.Debug.WriteLine("=========================");
        }
    }
}