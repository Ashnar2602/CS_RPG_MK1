using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CS_RPG_MK1.Services
{
    public class OpenRouterService
    {
        private readonly HttpClient _httpClient;
        private string _apiKey;

        // URL di base per le chiamate all'API OpenRouter
        private const string BaseUrl = "https://openrouter.ai/api/v1/";
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

            // Adattamento del metodo secondo i documenti di OpenRouter
            var requestBody = new
            {
                model = model ?? "deepseek/deepseek-chat-v3-0324:free", // Modello predefinito secondo i documenti
                messages = new[]
                {
                    new { role = "system", content = "Sei un Dungeon Master per una partita di un gioco basato su SRD5.1, rispondi sempre in italiano. Non troncare mai le frasi. Non superare i limiti di lunghezza richiesti nel prompt." },
                    new { role = "user", content = prompt }
                },
                max_tokens = 400, // Limite di token per la risposta
                temperature = 0.7 // Temperatura per la generazione della risposta
            };

            string responseContent = string.Empty; // Inizializzazione della variabile
            try
            {
                // Effettuiamo la chiamata API
                string json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("chat/completions", content);


                responseContent = await response.Content.ReadAsStringAsync();

                // Controlla il codice di stato HTTP
                var statusCode = response.StatusCode;
                if (!response.IsSuccessStatusCode)
                {
                    return $"Errore HTTP: {response.StatusCode}. Contenuto della risposta: {responseContent}";
                }

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

        public async Task CreateCharacterAsync()
        {
            if (!IsConfigured)
            {
                throw new InvalidOperationException("API key non configurata. Configura l'API key prima di effettuare richieste.");
            }

            var characterData = new Dictionary<string, string>();
            string[] steps = { "razza", "sottorazza", "classe", "sottoclasse", "tratti" };

            foreach (var step in steps)
            {
                string prompt = $"Guidami nella scelta della {step} per il mio personaggio. Elenca tutte le opzioni disponibili e chiedimi di scegliere.";

                string response = await GetResponseAsync(prompt);

                Console.WriteLine(response); // Mostra la risposta al giocatore (da sostituire con l'interfaccia grafica)

                Console.WriteLine($"Inserisci la tua scelta per {step}:");
                string userChoice = Console.ReadLine(); // Da sostituire con input dell'interfaccia grafica

                characterData[step] = userChoice;
            }

            // Riepilogo finale
            string summaryPrompt = "Genera un riepilogo dettagliato del personaggio basato sulle seguenti scelte: \n" +
                                   string.Join("\n", characterData.Select(kv => $"{kv.Key}: {kv.Value}"));

            string summary = await GetResponseAsync(summaryPrompt);

            Console.WriteLine(summary); // Mostra il riepilogo al giocatore (da sostituire con l'interfaccia grafica)

            // Salva il riepilogo su file
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "character_summary.json");
            await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(characterData, new JsonSerializerOptions { WriteIndented = true }));

            Console.WriteLine($"Riepilogo salvato in: {filePath}");
        }

        public void PopulateCharacterInfo()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "character_summary.json");

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Il file del riepilogo del personaggio non esiste. Crea un personaggio prima di visualizzarlo.");
                return;
            }

            string jsonContent = File.ReadAllText(filePath);
            var characterData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

            if (characterData == null)
            {
                Console.WriteLine("Errore nel caricamento dei dati del personaggio.");
                return;
            }

            // Popola l'area 2 con i dati del personaggio
            foreach (var entry in characterData)
            {
                Console.WriteLine($"{entry.Key}: {entry.Value}"); // Sostituire con il codice per aggiornare l'interfaccia grafica
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