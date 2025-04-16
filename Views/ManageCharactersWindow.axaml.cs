using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CS_RPG_MK1.Views
{
    public partial class ManageCharactersWindow : Window
    {
        public ManageCharactersWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}