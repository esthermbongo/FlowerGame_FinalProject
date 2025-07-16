using FlowerGame.Model;
using System;
using System.Linq;
using System.Text;
using Microsoft.Maui;

namespace FlowerGame.Services
{
    public class GameService
    {
        public GameState InitializeGame(string word, string language)
        {
            if (string.IsNullOrWhiteSpace(word))
                throw new ArgumentException("Word cannot be null or empty.", nameof(word));

            return new GameState
            {
                CurrentWord = word.ToUpperInvariant(),
                SelectedLanguage = language,
                DisplayWord = new string('_', word.Length),
                FlowerImagePath = "flower_full.png"
            };
        }

        public GameState ProcessGuess(GameState gameState, char letter)
        {
            letter = char.ToUpperInvariant(letter);

            if (gameState.GuessedLetters.Contains(letter))
                return gameState;

            var newState = CloneGameState(gameState);
            newState.GuessedLetters.Add(letter);

            if (newState.CurrentWord.Contains(letter))
            {
                var displayBuilder = new StringBuilder(newState.DisplayWord);
                for (int i = 0; i < newState.CurrentWord.Length; i++)
                {
                    if (newState.CurrentWord[i] == letter)
                        displayBuilder[i] = letter;
                }
                newState.DisplayWord = displayBuilder.ToString();

                if (!newState.DisplayWord.Contains('_'))
                {
                    newState.IsGameOver = true;
                    newState.IsWinner = true;
                }
            }
            else
            {
                newState.WrongLetters.Add(letter);
                newState.WrongGuesses++;
                newState.FlowerImagePath = GetFlowerImagePath(newState.WrongGuesses);

                if (newState.WrongGuesses >= newState.MaxWrongGuesses)
                {
                    newState.IsGameOver = true;
                    newState.IsWinner = false;
                }
            }

            return newState;
        }

        private GameState CloneGameState(GameState state)
        {
            return new GameState
            {
                CurrentWord = state.CurrentWord,
                SelectedLanguage = state.SelectedLanguage,
                DisplayWord = state.DisplayWord,
                FlowerImagePath = state.FlowerImagePath,
                GuessedLetters = new List<char>(state.GuessedLetters),
                WrongLetters = new List<char>(state.WrongLetters),
                WrongGuesses = state.WrongGuesses,
                MaxWrongGuesses = state.MaxWrongGuesses,
                IsGameOver = state.IsGameOver,
                IsWinner = state.IsWinner
            };
        }

        private string GetFlowerImagePath(int wrongGuesses)
        {
            return "flower_full.png";
        }

    }
}