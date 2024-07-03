using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
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
        DatePicker datePicker = this.FindControl<DatePicker>(DateControlName)!;
        datePicker.SelectedDate = DateTimeOffset.Now;

        Settings settings = SettingsService.LoadSettings();
        this.FindControl<TextBox>(TemplateDirTextBoxName)!.Text = settings.TemplateDir;
        this.FindControl<TextBox>(OutputDirTextBoxName)!.Text = settings.OutputDir;
    }

    private async void OnGenerateCertificateClickAsync(object sender, RoutedEventArgs e)
    {
        ViewModel model = GetViewModel();
        if (!await ValidateViewModelAsync(model).ConfigureAwait(false))
        {
            return;
        }

        this.Cursor = new Cursor(StandardCursorType.Wait);

        string certificateDir;
        try
        {
            certificateDir = await Task.Run(() => GenerateCertificate(model));
        }
        finally
        {
            this.Cursor = Cursor.Default;
        }

        await MessageBox.Show(this, $"Certificate generated in {certificateDir}");

        SaveSettings(model);
    }

    private static string GenerateCertificate(ViewModel model)
    {
        AdoptionRecord adoptionRecord = CreateAdoptionRecordFromViewModel(model);

        using var templateBitmapRetriever = new FileTemplateBitmapRetriever(model.TemplateDir!);
        var certificateGenerator = new CertificateGenerator(templateBitmapRetriever);
        return CertificateUtils.GenerateCertificate(adoptionRecord, certificateGenerator, model.OutputDir!);
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

    private async Task<bool> ValidateViewModelAsync(ViewModel model)
    {
        if (string.IsNullOrEmpty(model.TemplateDir))
        {
            await MessageBox.Show(this, "Please fill in the template folder.");
            return false;
        }

        if (!Directory.Exists(model.TemplateDir))
        {
            await MessageBox.Show(this, "Template folder does not exist.");
            return false;
        }

        if (string.IsNullOrEmpty(model.OutputDir))
        {
            await MessageBox.Show(this, "Please fill in the output folder.");
            return false;
        }

        if (!Directory.Exists(model.OutputDir))
        {
            await MessageBox.Show(this, "Output folder does not exist.");
            return false;
        }

        if (string.IsNullOrEmpty(model.FullName))
        {
            await MessageBox.Show(this, "Please fill in a name.");
            return false;
        }

        if (!int.TryParse(model.AreaM2Text, out int squareMeters))
        {
            await MessageBox.Show(this, "Please enter a valid number for square meters.");
            return false;
        }

        if (model.Date == null)
        {
            await MessageBox.Show(this, "Please select a date.");
            return false;
        }

        if (model.LanguageText == null)
        {
            await MessageBox.Show(this, "Please select a language.");
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

    private static void SaveSettings(ViewModel model)
    {
        Settings settings = new(model.TemplateDir!, model.OutputDir!);
        SettingsService.SaveSettings(settings);
    }
}