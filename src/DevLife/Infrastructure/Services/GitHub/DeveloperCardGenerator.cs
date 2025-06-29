using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using DevLife.Features.Auth.GitHub;
using DevLife.Features.RepositoryAnalyze;

namespace DevLife.Infrastructure.Services.GitHub;

public class DeveloperCardGenerator
{
    public byte[] GenerateCard(GetRepositoryAnalyze.DeveloperAnalysisResult input)
    {
        using var bitmap = new Bitmap(800, 600);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.White);

        var titleFont = new Font("Arial", 24, FontStyle.Bold);
        var textFont = new Font("Arial", 14);
        var blackBrush = Brushes.Black;

        int y = 20;
        graphics.DrawString($"👤 {input.PersonalityType}", titleFont, blackBrush, 20, y);
        y += 50;

        y = DrawTextBlock(graphics, "📋 Description:", input.CardDescription, textFont, blackBrush, y);
        y = DrawTextBlock(graphics, "✅ Strengths:", string.Join(", ", input.Strengths), textFont, blackBrush, y);
        y = DrawTextBlock(graphics, "⚠️ Weaknesses:", string.Join(", ", input.Weaknesses), textFont, blackBrush, y);

        graphics.DrawString("🌟 Celebrity Developers:", textFont, blackBrush, 20, y);
        y += 25;
        foreach (var celeb in input.CelebrityDevelopers)
        {
            graphics.DrawString($"• {celeb}", textFont, blackBrush, 40, y);
            y += 25;
        }

        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }

    private int DrawTextBlock(Graphics graphics, string title, string content, Font font, Brush brush, int startY)
    {
        graphics.DrawString(title, font, brush, 20, startY);
        startY += 25;
        var lines = WrapText(content, font, graphics, 740);
        foreach (var line in lines)
        {
            graphics.DrawString(line, font, brush, 40, startY);
            startY += 20;
        }
        startY += 10;
        return startY;
    }

    private List<string> WrapText(string text, Font font, Graphics g, int maxWidth)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        var line = new StringBuilder();

        foreach (var word in words)
        {
            var testLine = line.Length == 0 ? word : line + " " + word;
            var size = g.MeasureString(testLine, font);
            if (size.Width > maxWidth)
            {
                lines.Add(line.ToString());
                line.Clear();
                line.Append(word);
            }
            else
            {
                if (line.Length > 0) line.Append(" ");
                line.Append(word);
            }
        }

        if (line.Length > 0)
            lines.Add(line.ToString());

        return lines;
    }
}