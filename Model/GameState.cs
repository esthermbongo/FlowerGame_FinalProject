using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerGame.Model
{
    public class GameState
    {
        public string CurrentWord { get; set; } = string.Empty;
        public string DisplayWord { get; set; } = string.Empty;
        public List<char> GuessedLetters { get; set; } = new();
        public List<char> WrongLetters { get; set; } = new();
        public int WrongGuesses { get; set; } = 0;
        public int MaxWrongGuesses { get; set; } = 6;
        public bool IsGameOver { get; set; } = false;
        public bool IsWinner { get; set; } = false;
        public string SelectedLanguage { get; set; } = string.Empty;
        public string FlowerImagePath { get; set; } = "flower_full.png";
    }
}
