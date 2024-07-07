using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CertificateGeneratorCore;

namespace BulkToolUI;

public partial class MainWindow : Window
{
    const string TemplateDirTextBoxName = "TemplateDirTextBox";
    const string OutputDirTextBoxName = "OutputDirTextBox";
    const string ExcelFileTextBoxName = "ExcelFileTextBox";
    const string ProgressTextBlockName = "ProgressTextBlock";

    public class ViewModel
    {
        public string? TemplateDir { get; set; }
        public string? OutputDir { get; set; }
        public string? ExcelFile { get; set; }
    }

    public MainWindow()
    {
        InitializeComponent();
        SetDefaultValues();
    }

    private void SetDefaultValues()
    {
        Settings settings = SettingsService.LoadSettings();
        this.FindControl<TextBox>(TemplateDirTextBoxName)!.Text = settings.TemplateDir;
        this.FindControl<TextBox>(OutputDirTextBoxName)!.Text = settings.OutputDir;
    }

    private async void OnGenerateCertificatesClickAsync(object sender, RoutedEventArgs e)
    {
        ViewModel model = GetViewModel();
        if (!await ValidateViewModelAsync(model).ConfigureAwait(false))
        {
            return;
        }

        Cursor = new Cursor(StandardCursorType.Wait);

        int nrRecords;
        try
        {
            nrRecords = await Task.Run(() => GenerateCertificates(model));
        }
        finally
        {
            Cursor = Cursor.Default;
        }

        await MessageBox.Show(this, $"{nrRecords} certificates were generated.");

        SaveSettings(model);
    }

    private int GenerateCertificates(ViewModel model)
    {
        using var templateBitmapRetriever = new FileTemplateBitmapRetriever(model.TemplateDir!);
        var certificateGenerator = new CertificateGenerator(templateBitmapRetriever);
        List<AdoptionRecord> adoptionRecords = CertificateUtils.ParseExcel(model.ExcelFile!).ToList();
        int currentRecord = 1;
        foreach (AdoptionRecord adoptionRecord in adoptionRecords)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                TextBlock progressTextBlock = this.FindControl<TextBlock>(ProgressTextBlockName)!;
                progressTextBlock.Text = $"{currentRecord} / {adoptionRecords.Count}";
            });

            CertificateUtils.GenerateCertificate(adoptionRecord, certificateGenerator, model.OutputDir!);
            currentRecord++;
        }

        return adoptionRecords.Count;
    }

    private ViewModel GetViewModel()
    {
        ViewModel model = new()
        {
            TemplateDir = this.FindControl<TextBox>(TemplateDirTextBoxName)!.Text,
            OutputDir = this.FindControl<TextBox>(OutputDirTextBoxName)!.Text,
            ExcelFile = this.FindControl<TextBox>(ExcelFileTextBoxName)!.Text,
        };

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

        if (string.IsNullOrEmpty(model.ExcelFile))
        {
            await MessageBox.Show(this, "Please fill in the Excel file.");
            return false;
        }

        if (!File.Exists(model.ExcelFile))
        {
            await MessageBox.Show(this, "Excel file does not exist.");
            return false;
        }

        return true;
    }

    private static void SaveSettings(ViewModel model)
    {
        Settings settings = new(model.TemplateDir!, model.OutputDir!);
        SettingsService.SaveSettings(settings);
    }
}