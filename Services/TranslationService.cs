using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.IO;

namespace FlowerGame.Services
{
    public class TranslationService
    {
        private readonly HttpClient httpClient;
        private readonly string subscriptionKey;
        private readonly string region;
        private const string endpoint = "https://api.cognitive.microsofttranslator.com";

        public TranslationService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            subscriptionKey = ReadApiKeyFromSettings();
            region = "eastus";
        }

        private string ReadApiKeyFromSettings()
        {
            try
            {
                using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;
                using var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();

                using var document = JsonDocument.Parse(json);
                var translationService = document.RootElement.GetProperty("TranslationService");
                var subscriptionKey = translationService.GetProperty("SubscriptionKey").GetString();

                if (!string.IsNullOrEmpty(subscriptionKey))
                {
                    return subscriptionKey;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing JSON: {ex.Message}");
            }

            throw new InvalidOperationException("Please add your API key to appsettings.json");
        }

        public async Task<string> TranslateAsync(string text, string targetLanguage)
        {
            if (targetLanguage.Equals("english", StringComparison.OrdinalIgnoreCase))
                return text;

            try
            {
                await Task.Delay(200).ConfigureAwait(false);

                string route = $"/translate?api-version=3.0&from=en&to={GetLanguageCode(targetLanguage)}";
                string requestUri = endpoint + route;

                object[] body = new object[] { new { Text = text } };
                var requestBody = JsonSerializer.Serialize(body);

                using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
                };
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", region);

                var response = await httpClient.SendAsync(request).ConfigureAwait(false);
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseBody);
                    var root = doc.RootElement;
                    if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                    {
                        var firstResult = root[0];
                        if (firstResult.TryGetProperty("translations", out var translations) &&
                            translations.ValueKind == JsonValueKind.Array &&
                            translations.GetArrayLength() > 0)
                        {
                            var firstTranslation = translations[0];
                            if (firstTranslation.TryGetProperty("text", out var textElement))
                            {
                                return textElement.GetString() ?? text;
                            }
                        }
                    }
                    return text;
                }
                return text;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Translation error: {ex.Message}");
                return text;
            }
        }

        private string GetLanguageCode(string language) => language.ToLowerInvariant() switch
        {
            "spanish" => "es",
            "french" => "fr",
            "english" => "en",
            _ => "en"
        };
    }
}