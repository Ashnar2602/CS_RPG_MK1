using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Linq;
using CS_RPG_MK1.Services;
using CS_RPG_MK1.Views;

namespace CS_RPG_MK1;

public partial class MainWindow : Window
{
    private StringBuilder _chatHistory = new StringBuilder();
    private readonly OpenRouterService _openRouterService;
    private string? _currentApiKey;

    public MainWindow()
    {
        InitializeComponent();
        _openRouterService = new OpenRouterService();
        LoadCharacterInfo();
        
        // Visualizza un messaggio iniziale che indica la necessità di configurare l'API key
        if (!_openRouterService.IsConfigured)
        {
            AIResponseTextBlock.Text = "Benvenuto nel Gioco di Ruolo!\n\nPer iniziare, configura la tua API key di OpenRouter utilizzando il pulsante 'Configura API Key' nell'area impostazioni.";
        }
    }

    private async void SendButton_Click(object sender, RoutedEventArgs e)
    {
        await ProcessUserInput();
    }

    private async void OnUserInputKeyDown(object sender, KeyEventArgs e)
    {
        // Invia il messaggio premendo Ctrl+Enter oppure Shift+Enter
        if (e.Key == Key.Enter && (e.KeyModifiers == KeyModifiers.Control || e.KeyModifiers == KeyModifiers.Shift))
        {
            e.Handled = true;
            await ProcessUserInput();
        }
    }

    private async void ConfigApiButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ApiKeyDialog(_currentApiKey ?? string.Empty);
        var result = await dialog.ShowDialog<bool>(this);

        if (result)
        {
            _currentApiKey = dialog.ApiKey;
            _openRouterService.SetApiKey(_currentApiKey);
            
            if (_openRouterService.IsConfigured)
            {
                // Se è la prima configurazione e non ci sono ancora messaggi, mostriamo un messaggio di benvenuto
                if (_chatHistory.Length == 0)
                {
                    _chatHistory.AppendLine("AI: Benvenuto nel mondo di Aetheria! Sono il Maestro di Gioco e ti guiderò in questa avventura. Cosa desideri fare?");
                    AIResponseTextBlock.Text = _chatHistory.ToString();
                }
            }
        }
    }

    private async Task ProcessUserInput()
    {
        string userInput = UserInputTextBox?.Text?.Trim() ?? string.Empty;
        
        if (string.IsNullOrWhiteSpace(userInput))
            return;

        // Verifica che l'API key sia configurata
        if (!_openRouterService.IsConfigured)
        {
            // Mostra un messaggio all'utente che deve configurare l'API key
            _chatHistory.AppendLine("Sistema: Per favore, configura prima la tua API key di OpenRouter.");
            AIResponseTextBlock.Text = _chatHistory.ToString();
            return;
        }

        // Aggiorna la cronologia della chat con l'input dell'utente
        _chatHistory.AppendLine("Tu: " + userInput);
        
        // Svuota il campo di input
        var userInputTextBox = UserInputTextBox;
        if (userInputTextBox != null)
        {
            userInputTextBox.Text = string.Empty;
        }
        
        // Aggiorna l'interfaccia per mostrare subito l'input dell'utente
        AIResponseTextBlock.Text = _chatHistory.ToString();

        // Mostra un indicatore di caricamento
        _chatHistory.AppendLine("AI: Sto pensando...");
        AIResponseTextBlock.Text = _chatHistory.ToString();

        try
        {
            // Ottieni una risposta dall'AI
            string aiResponse = await _openRouterService.GetResponseAsync(userInput);
            
            // Rimuovi l'indicatore di caricamento e aggiungi la risposta effettiva
            _chatHistory.Remove(_chatHistory.Length - "AI: Sto pensando...".Length - 2, "AI: Sto pensando...".Length + 2);
            _chatHistory.AppendLine("AI: " + aiResponse);
            
            // Aggiungi una riga vuota per separare i messaggi
            _chatHistory.AppendLine();
        }
        catch (Exception ex)
        {
            // In caso di errore, mostra un messaggio appropriato
            _chatHistory.Remove(_chatHistory.Length - "AI: Sto pensando...".Length - 2, "AI: Sto pensando...".Length + 2);
            _chatHistory.AppendLine($"Sistema: Si è verificato un errore nella comunicazione con l'AI: {ex.Message}");
            _chatHistory.AppendLine();
        }

        // Aggiorna l'interfaccia utente
        AIResponseTextBlock.Text = _chatHistory.ToString();
    }

    private void LoadCharacterInfo()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "character_summary.json");

        if (!File.Exists(filePath))
        {
            // Mostra un messaggio nell'interfaccia grafica se il file non esiste
            var characterInfoBlock = this.FindControl<TextBlock>("CharacterInfoBlock");
            if (characterInfoBlock != null)
            {
                characterInfoBlock.Text = "Nessun personaggio trovato. Crea un personaggio per visualizzarlo.";
            }
            return;
        }

        string jsonContent = File.ReadAllText(filePath);
        var characterData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

        if (characterData == null)
        {
            var characterInfoBlock = this.FindControl<TextBlock>("CharacterInfoBlock");
            if (characterInfoBlock != null)
            {
                characterInfoBlock.Text = "Errore nel caricamento dei dati del personaggio.";
            }
            return;
        }

        // Popola l'area 2 con i dati del personaggio
        var characterInfo = string.Join("\n", characterData.Select(kv => $"{kv.Key}: {kv.Value}"));
        var characterInfoBlockFinal = this.FindControl<TextBlock>("CharacterInfoBlock");
        if (characterInfoBlockFinal != null)
        {
            characterInfoBlockFinal.Text = characterInfo;
        }
    }

    private void CreateCharacterButton_Click(object sender, RoutedEventArgs e)
    {
        var createCharacterWindow = new CreateCharacterWindow();
        createCharacterWindow.ShowDialog(this);
    }
}