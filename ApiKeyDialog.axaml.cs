using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace CS_RPG_MK1
{
    public partial class ApiKeyDialog : Window
    {
        public string ApiKey { get; private set; }

        public ApiKeyDialog()
        {
            InitializeComponent();
        }

        public ApiKeyDialog(string currentApiKey) : this()
        {
            if (!string.IsNullOrEmpty(currentApiKey))
            {
                ApiKeyTextBox.Text = currentApiKey;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            ApiKeyTextBox = this.FindControl<TextBox>("ApiKeyTextBox");
        }

        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ApiKey = ApiKeyTextBox.Text;
            Close(true);
        }

        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close(false);
        }
    }
}