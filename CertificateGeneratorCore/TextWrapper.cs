using SkiaSharp;

namespace CertificateGeneratorCore;

public static class TextWrapper
{
    public static IList<string> WrapText(string text, SKPaint paint, int maxWidth)
    {
        List<string> wrappedLines = [];
        string[] parts = text.Split(' ');
        int nrParts = parts.Length;
        int currentPart = 0;

        while (currentPart < nrParts)
        {
            int nrPartsOnThisLine = GetNrPartsThatFit(parts, currentPart, paint, maxWidth);
            string currentLine = string.Join(' ', parts.Skip(currentPart).Take(nrPartsOnThisLine));
            wrappedLines.Add(currentLine);
            currentPart += nrPartsOnThisLine;
        }

        return wrappedLines;
    }

    private static int GetNrPartsThatFit(string[] parts, int currentPart, SKPaint paint, int maxWidth)
    {
        string currentLine = parts[currentPart];

        if (currentPart >= parts.Length)
        {
            return 0;
        }

        int nrPartsThatFit = 1;
        if (currentPart == parts.Length - 1)
        {
            return nrPartsThatFit;
        }

        string nextPart = parts[currentPart + 1];
        string textToCheck = $"{currentLine} {nextPart}";
        while (paint.MeasureText(textToCheck) <= maxWidth)
        {
            currentLine = textToCheck;
            currentPart++;
            nrPartsThatFit++;

            if (currentPart == parts.Length - 1)
            {
                return nrPartsThatFit;
            }

            nextPart = parts[currentPart + 1];
            textToCheck = $"{currentLine} {nextPart}";
        }

        return nrPartsThatFit;
    }
}