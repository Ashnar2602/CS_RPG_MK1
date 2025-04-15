using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CS_RPG_MK1.Services
{
    public class OpenRouterService
    {
        private readonly HttpClient _httpClient;
        private string _apiKey;
        
        // URL di base per le chiamate all'API OpenRouter
        private const string BaseUrl = "https://openrouter.ai/api/v1";
        private const string DefaultModel = "deepseek/deepseek-chat-v3-0324:free";

        public OpenRouterService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public void SetApiKey(string apiKey)
        {
            _apiKey = apiKey;
            // Configura gli header per le successive chiamate API
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://CS_RPG_MK1");  // Può essere modificato con il reale URL dell'app
            _httpClient.DefaultRequestHeaders.Add("X-Title", "CS_RPG_MK1");  // Titolo dell'applicazione
        }

        public bool IsConfigured => !string.IsNullOrEmpty(_apiKey);

        public async Task<string> GetResponseAsync(string prompt, string context = "", string model = null)
        {
            if (!IsConfigured)
            {
                throw new InvalidOperationException("API key non configurata. Configura l'API key prima di effettuare richieste.");
            }

            // Prepariamo il contesto del gioco di ruolo
            string systemPrompt = "Sei un game master esperto di giochi di ruolo fantasy. Rispondi in modo dettagliato e coinvolgente, " +
                                 "creando un'atmosfera immersiva. Non menzionare mai che sei un'IA. Parla come un narratore di " +
                                 "un mondo fantasy medievale. Mantieni le risposte concise ma ricche di dettagli ambientali.";

            if (!string.IsNullOrEmpty(context))
            {
                systemPrompt += " " + context;
            }

            // Prepariamo la richiesta
            var requestBody = new
            {
                model = model ?? DefaultModel,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = prompt }
                },
                max_tokens = 500
            };

            string responseContent = string.Empty; // Inizializzazione della variabile
            try
            {
                // Effettuiamo la chiamata API
                var response = await _httpClient.PostAsJsonAsync("/chat/completions", requestBody);
                response.EnsureSuccessStatusCode();

                // Leggiamo la risposta
                responseContent = await response.Content.ReadAsStringAsync();

                // Controlla se la risposta è in formato JSON
                if (!response.Content.Headers.ContentType?.MediaType?.Contains("application/json") ?? true)
                {
                    return $"Errore: la risposta non è in formato JSON. Contenuto della risposta: {responseContent}";
                }

                var deserializedResponse = JsonSerializer.Deserialize<OpenRouterResponse>(responseContent);

                return deserializedResponse?.Choices?[0]?.Message?.Content ?? 
                       "Non sono riuscito a interpretare la risposta dell'API.";
            }
            catch (JsonException jsonEx)
            {
                return $"Errore nel parsing della risposta JSON: {jsonEx.Message}. Contenuto della risposta: {responseContent}";
            }
            catch (HttpRequestException e)
            {
                return $"Errore nella comunicazione con l'API: {e.Message}";
            }
            catch (Exception e)
            {
                return $"Errore inaspettato: {e.Message}";
            }
        }

        // Classi per la deserializzazione della risposta
        private class OpenRouterResponse
        {
            [JsonPropertyName("choices")]
            public List<OpenRouterChoice> Choices { get; set; }
        }

        private class OpenRouterChoice
        {
            [JsonPropertyName("message")]
            public OpenRouterMessage Message { get; set; }
        }

        private class OpenRouterMessage
        {
            [JsonPropertyName("content")]
            public string Content { get; set; }
        }
    }
}