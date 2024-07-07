namespace BulkToolUI;

public class Settings(string templateDir, string outputDir)
{
    public string TemplateDir { get; } = templateDir;
    public string OutputDir { get; } = outputDir;
}
