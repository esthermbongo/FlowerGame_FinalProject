using FlowerGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FlowerGame.Services
{
    public class WordService
    {
        private static readonly HttpClient httpClient = new();
        private readonly TranslationService translationService;

        public WordService(TranslationService translationService)
        {
            this.translationService = translationService;
        }

        private class DatamuseWord
        {
            public string Word { get; set; }
            public int? Score { get; set; }
            public List<string>? Tags { get; set; }
        }

        public async Task<List<Word>> GetWords(string language, string category, string level, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = category?.ToLowerInvariant() switch
                {
                    "animal" => "https://api.datamuse.com/words?ml=animal&max=100",
                    "food" => "https://api.datamuse.com/words?rel_jjb=food&max=100",
                    "plant" => "https://api.datamuse.com/words?ml=plant&rel_jjb=tree&max=100",
                    "person" => "https://api.datamuse.com/words?rel_jjb=person&max=100",
                    _ => "https://api.datamuse.com/words?ml=general&max=100"
                };

                var response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    return await GetTranslatedFallbackWords(level, category, language).ConfigureAwait(false);

                var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                var datamuseWords = JsonSerializer.Deserialize<List<DatamuseWord>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (datamuseWords == null || datamuseWords.Count == 0)
                    return await GetTranslatedFallbackWords(level, category, language).ConfigureAwait(false);

                var words = datamuseWords
                    .Select(dw => new Word
                    {
                        WordText = dw.Word?.ToUpperInvariant() ?? "",
                        Score = dw.Score,
                        Tags = dw.Tags ?? new List<string>()
                    })
                    .Where(w => !string.IsNullOrEmpty(w.WordText))
                    .ToList();

                words = level?.ToLowerInvariant() switch
                {
                    "easy" => words.Where(w => w.WordText.Length <= 5).ToList(),
                    "medium" => words.Where(w => w.WordText.Length > 5 && w.WordText.Length <= 8).ToList(),
                    "hard" => words.Where(w => w.WordText.Length > 8).ToList(),
                    _ => words
                };

                words = words.Take(5).ToList();

                if (words.Count > 0)
                {
                    if (!language.Equals("english", StringComparison.OrdinalIgnoreCase))
                    {
                        for (int i = 0; i < words.Count; i++)
                        {
                            var translatedWord = await translationService.TranslateAsync(words[i].WordText, language).ConfigureAwait(false);
                            words[i].WordText = translatedWord.ToUpperInvariant();
                        }
                    }
                    return words;
                }
                else
                {
                    return await GetTranslatedFallbackWords(level, category, language).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching words: {ex.Message}");
                return await GetTranslatedFallbackWords(level, category, language).ConfigureAwait(false);
            }
        }

        private async Task<List<Word>> GetTranslatedFallbackWords(string level, string category, string language)
        {
            var words = GetFallbackWords(level, category);
            if (!language.Equals("english", StringComparison.OrdinalIgnoreCase))
            {
                for (int i = 0; i < words.Count; i++)
                {
                    var translatedWord = await translationService.TranslateAsync(words[i].WordText, language).ConfigureAwait(false);
                    words[i].WordText = translatedWord.ToUpperInvariant();
                }
            }
            return words;
        }

        private static readonly Dictionary<string, Dictionary<string, List<Word>>> FallbackWords = new()
        {
            ["animal"] = new()
            {
                ["easy"] = new() { new Word("CAT"), new Word("DOG"), new Word("BIRD"), new Word("FISH"), new Word("FROG") },
                ["medium"] = new() { new Word("RABBIT"), new Word("TURTLE"), new Word("MONKEY"), new Word("TIGER") },
                ["hard"] = new() { new Word("RHINOCEROS"), new Word("HIPPOPOTAMUS"), new Word("CHIMPANZEE") }
            },
            ["food"] = new()
            {
                ["easy"] = new() { new Word("APPLE"), new Word("BREAD"), new Word("CAKE"), new Word("RICE"), new Word("SOUP") },
                ["medium"] = new() { new Word("BANANA"), new Word("CHEESE"), new Word("BURGER"), new Word("PIZZA") },
                ["hard"] = new() { new Word("SPAGHETTI"), new Word("HAMBURGER"), new Word("STRAWBERRY") }
            },
            ["plant"] = new()
            {
                ["easy"] = new() { new Word("TREE"), new Word("ROSE"), new Word("LEAF"), new Word("VINE"), new Word("HERB") },
                ["medium"] = new() { new Word("FLOWER"), new Word("CACTUS"), new Word("BAMBOO"), new Word("MAPLE"), new Word("GARDEN") },
                ["hard"] = new() { new Word("SUNFLOWER"), new Word("DANDELION"), new Word("EUCALYPTUS") }
            },
            ["person"] = new()
            {
                ["easy"] = new() { new Word("MAN"), new Word("WOMAN"), new Word("CHILD"), new Word("BABY"), new Word("ADULT") },
                ["medium"] = new() { new Word("MOTHER"), new Word("FATHER"), new Word("SISTER"), new Word("FRIEND") },
                ["hard"] = new() { new Word("GRANDMOTHER"), new Word("GRANDFATHER"), new Word("TEENAGER") }
            }
        };

        private List<Word> GetFallbackWords(string level, string category)
        {
            var catKey = (category ?? "animal").ToLowerInvariant();
            var levelKey = (level ?? "easy").ToLowerInvariant();
            if (FallbackWords.TryGetValue(catKey, out var levelDict) && levelDict.TryGetValue(levelKey, out var words))
                return new List<Word>(words);
            return new List<Word> { new Word("WORD") };
        }
    }
}