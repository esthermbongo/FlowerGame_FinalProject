# 🌸 Flower Hangman Game

A cute multilingual word-guessing game built with .NET MAUI! Save your adorable flower by guessing the word before all the petals fall away.

## 🎮 Features

- 🌍 **Multi-language support** - Play in English, Spanish, or French with real translation
- 🌸 **Adorable flower** - Watch cute animations as you play
- 💡 **Smart hint system** - Get helpful clues when you're stuck
- 🎯 **Multiple difficulty levels** - Choose from Easy, Medium, and Hard words
- 🏷️ **Category selection** - Pick from Animals, Food, Plants, or People
- 💖 **Lives counter** - Track your remaining chances
- 🎨 **Cute pink theme** - Beautiful, friendly interface designed for all ages

## 🎲 How to Play

1. **Choose your settings:**
   - Select your preferred language (English, Spanish, French)
   - Pick a difficulty level (Easy, Medium, Hard)
   - Choose a category (Animal, Food, Plant, Person)

2. **Start guessing:**
   - Click letter buttons to guess letters in the word
   - Correct guesses reveal letters in the word
   - Wrong guesses make your flower sad and cost you a life

3. **Use hints wisely:**
   - Click the hint button for helpful clues
   - Each word gets one hint

4. **Win or lose:**
   - Win by guessing the complete word
   - Lose if you make 6 wrong guesses

## 🛠️ Technologies Used

- **.NET MAUI** - Cross-platform mobile and desktop UI framework
- **C#** - Primary programming language
- **MVVM Pattern** - Clean separation of UI and business logic
- **Microsoft Translator API** - Real-time translation between languages
- **Datamuse API** - Word generation and categorization
- **Cute animations** - Smooth transitions and feedback

## 📱 Supported Platforms

- ✅ Windows

## 🚀 Setup Instructions

### Prerequisites
- Visual Studio 2022 with .NET MAUI workload
- Microsoft Azure account (for translation API)

### Installation
1. **Clone this repository:**
git clone https://github.com/YOUR_USERNAME/flower-hangman-game.git

2. **Set up API key:**
- Copy `appsettings.example.json` to `appsettings.json`
- Get a free API key from [Microsoft Translator](https://docs.microsoft.com/azure/cognitive-services/translator/)
- Add your API key to `appsettings.json`:
```json
{
  "TranslationService": {
    "SubscriptionKey": "YOUR_API_KEY_HERE",
    "Region": "eastus"
  }
}

🌟 What I Learned
This project helped me learn:

Cross-platform development with .NET MAUI
MVVM architecture and data binding
API integration and async programming
Multilingual app development
Animation and user experience design
Git and GitHub workflow

🤝 Contributing
Feel free to open issues or submit pull requests! Some ideas for future features:

More languages (German, Italian, Portuguese)
Different flower types to choose from
Sound effects and music
Difficulty-based scoring system
Save game progress

📄 License
This project is open source and available under the MIT License.
🙏 Acknowledgments

Flower image created with AI generation tools
Microsoft Translator API for translation services
Datamuse API for word generation
The .NET MAUI community for excellent documentation


Made with 💖 and lots of cute pink styling!