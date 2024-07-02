using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace GenerateCertificateUI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnGenerateCertificateClick(object sender, RoutedEventArgs e)
    {
        string? fullName = this.FindControl<TextBox>("FullNameTextBox")!.Text;
        if (string.IsNullOrEmpty(fullName))
        {
            MessageBox.Show(this, "Please fill in a name.");
            return;
        }

        string? squareMetersText = this.FindControl<TextBox>("SquareMetersTextBox")!.Text;
        if (!int.TryParse(squareMetersText, out int squareMeters))
        {
            MessageBox.Show(this, "Please enter a valid number for square meters.");
            return;
        }

        DateTimeOffset? date = this.FindControl<DatePicker>("DatePicker")!.SelectedDate;
        if (date == null)
        {
            MessageBox.Show(this, "Please select a date.");
            return;
        }

        var languageComboBox = this.FindControl<ComboBox>("LanguageComboBox");
        ComboBoxItem? selectedLanguageItem = (ComboBoxItem?)languageComboBox!.SelectedItem;
        if (selectedLanguageItem == null)
        {
            MessageBox.Show(this, "Please select a language.");
            return;
        }

        string languageText = (string)selectedLanguageItem.Content!;
        var language = (CertificateGeneratorCore.Language)Enum.Parse(typeof(CertificateGeneratorCore.Language), languageText);

        var certificate = new CertificateModel
        {
            Name = fullName,
            SquareMeters = squareMeters,
            Date = date.GetValueOrDefault().DateTime,
            Language = language
        };

        // Handle the certificate generation logic here
        // For now, we'll just display a message box
        MessageBox.Show(this, $"Certificate generated for {certificate.Name}!");
    }
}