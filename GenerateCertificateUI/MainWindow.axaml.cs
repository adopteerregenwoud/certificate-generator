using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CertificateGeneratorCore;

namespace GenerateCertificateUI;

public partial class MainWindow : Window
{
    const string TemplateDirTextBoxName = "TemplateDirTextBox";
    const string OutputDirTextBoxName = "OutputDirTextBox";
    const string DateControlName = "DatePicker";
    const string FullNameTextBoxName = "FullNameTextBox";
    const string SquareMetersTextBoxName = "SquareMetersTextBox";
    const string LanguageComboBoxName = "LanguageComboBox";

    public class ViewModel
    {
        public string? TemplateDir { get; set; }
        public string? OutputDir { get; set; }
        public string? FullName { get; set; }
        public string? AreaM2Text { get; set; }
        public string? LanguageText { get; set; }
        public DateTimeOffset? Date { get; set; }
    }

    public MainWindow()
    {
        InitializeComponent();
        SetDefaultValues();
    }

    private void SetDefaultValues()
    {
        // Set the default date to today
        DatePicker datePicker = this.FindControl<DatePicker>(DateControlName)!;
        datePicker.SelectedDate = DateTimeOffset.Now;
    }

    private void OnGenerateCertificateClick(object sender, RoutedEventArgs e)
    {
        ViewModel model = GetViewModel();
        if (!ValidateViewModel(model))
        {
            return;
        }

        AdoptionRecord adoptionRecord = CreateAdoptionRecordFromViewModel(model);

        // Handle the certificate generation logic here
        // For now, we'll just display a message box
        MessageBox.Show(this, $"Certificate generated for {adoptionRecord.Name}!");
    }

    private ViewModel GetViewModel()
    {
        ViewModel model = new()
        {
            TemplateDir = this.FindControl<TextBox>(TemplateDirTextBoxName)!.Text,
            OutputDir = this.FindControl<TextBox>(OutputDirTextBoxName)!.Text,
            FullName = this.FindControl<TextBox>(FullNameTextBoxName)!.Text,
            AreaM2Text = this.FindControl<TextBox>(SquareMetersTextBoxName)!.Text,
            Date = this.FindControl<DatePicker>(DateControlName)!.SelectedDate,
        };

        ComboBox languageComboBox = this.FindControl<ComboBox>(LanguageComboBoxName)!;
        ComboBoxItem? selectedLanguageItem = (ComboBoxItem?)languageComboBox.SelectedItem;
        if (selectedLanguageItem != null)
        {
            model.LanguageText = (string?)selectedLanguageItem.Content;
        }

        return model;
    }

    private bool ValidateViewModel(ViewModel model)
    {
        if (string.IsNullOrEmpty(model.TemplateDir))
        {
            MessageBox.Show(this, "Please fill in the template folder.");
            return false;
        }

        if (string.IsNullOrEmpty(model.OutputDir))
        {
            MessageBox.Show(this, "Please fill in the output folder.");
            return false;
        }

        if (string.IsNullOrEmpty(model.FullName))
        {
            MessageBox.Show(this, "Please fill in a name.");
            return false;
        }

        if (!int.TryParse(model.AreaM2Text, out int squareMeters))
        {
            MessageBox.Show(this, "Please enter a valid number for square meters.");
            return false;
        }

        if (model.Date == null)
        {
            MessageBox.Show(this, "Please select a date.");
            return false;
        }

        if (model.LanguageText == null)
        {
            MessageBox.Show(this, "Please select a language.");
            return false;
        }

        return true;
    }

    private static AdoptionRecord CreateAdoptionRecordFromViewModel(ViewModel model)
    {
        return new AdoptionRecord
        {
            Name = model.FullName!,
            SquareMeters = int.Parse(model.AreaM2Text!),
            Date = DateOnly.FromDateTime(model.Date.GetValueOrDefault().DateTime),
            Language = Enum.Parse<Language>(model.LanguageText!)
        };
    }
}