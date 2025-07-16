using FlowerGame.ViewModel;

namespace FlowerGame.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(WordsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}