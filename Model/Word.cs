using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FlowerGame.Model
{
    public class Word : IEquatable<Word>
    {
        [JsonPropertyName("word")]
        public string WordText { get; set; } = string.Empty;

        [JsonPropertyName("score")]
        public int? Score { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new();

        public Word() { }

        public Word(string wordText, int? score = null, List<string>? tags = null)
        {
            WordText = wordText;
            Score = score;
            Tags = tags ?? new List<string>();
        }

        public override string ToString() => WordText;

        public override bool Equals(object? obj) => Equals(obj as Word);

        public bool Equals(Word? other)
        {
            if (other is null) return false;
            return WordText == other.WordText && Score == other.Score;
        }

        public override int GetHashCode() => HashCode.Combine(WordText, Score);

        public static bool operator ==(Word? left, Word? right) => EqualityComparer<Word>.Default.Equals(left, right);
        public static bool operator !=(Word? left, Word? right) => !(left == right);
    }
}

