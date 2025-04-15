namespace BulkToolUI;

public class Settings(string templateDir, string logoPath, string outputDir)
{
    public string TemplateDir { get; } = templateDir;
    public string LogoPath { get; } = logoPath;
    public string OutputDir { get; } = outputDir;
}
