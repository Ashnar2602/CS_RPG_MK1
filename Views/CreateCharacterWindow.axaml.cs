using System;
using System.Text.Json;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CS_RPG_MK1.Views
{
    public partial class CreateCharacterWindow : Window
    {
        public CreateCharacterWindow()
        {
            InitializeComponent();
        }

        private void OnConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            // Recupera le scelte dell'utente
            var selectedRace = (RaceComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
            var selectedSubrace = (SubraceComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
            var selectedClass = (ClassComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
            var selectedSubclass = (SubclassComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;

            // Recupera le statistiche dal sistema point buy
            var strength = (int)(StrengthInput.Value ?? 0);
            var dexterity = (int)(DexterityInput.Value ?? 0);
            var constitution = (int)(ConstitutionInput.Value ?? 0);
            var intelligence = (int)(IntelligenceInput.Value ?? 0);
            var wisdom = (int)(WisdomInput.Value ?? 0);
            var charisma = (int)(CharismaInput.Value ?? 0);

            // Validazione delle scelte
            if (string.IsNullOrEmpty(selectedRace) || string.IsNullOrEmpty(selectedClass))
            {
                MessageBox.Show("Per favore, seleziona almeno una razza e una classe.");
                return;
            }

            // Salva i dati del personaggio
            var characterData = new
            {
                Razza = selectedRace,
                Sottorazza = selectedSubrace,
                Classe = selectedClass,
                Sottoclasse = selectedSubclass,
                Statistiche = new
                {
                    Forza = strength,
                    Destrezza = dexterity,
                    Costituzione = constitution,
                    Intelligenza = intelligence,
                    Saggezza = wisdom,
                    Carisma = charisma
                }
            };

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "character_summary.json");
            File.WriteAllText(filePath, JsonSerializer.Serialize(characterData, new JsonSerializerOptions { WriteIndented = true }));

            // Chiudi la finestra
            this.Close();
        }
    }

    // Implementazione temporanea di MessageBox
    public static class MessageBox
    {
        public static void Show(string message)
        {
            Console.WriteLine(message);
        }
    }
}