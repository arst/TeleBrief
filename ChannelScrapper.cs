using System.Globalization;
using HtmlAgilityPack;

namespace TeleBrief;

public class ChannelScrapper
{
    private static readonly HttpClient HttpClient = new();

    public static async Task<List<string>> GetTodaysMessagesFromChannel(string channel)
    {
        var url = $"https://t.me/s/{channel}";
        var html = await HttpClient.GetStringAsync(url);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var messages = new List<string>();
        
        var messageNodes = doc.DocumentNode
            .SelectNodes("//div[contains(@class,'tgme_widget_message_wrap')]");

        if (messageNodes == null)
            return messages;

        foreach (var messageNode in messageNodes)
        {
            var textNode = messageNode.SelectSingleNode(".//div[contains(@class,'tgme_widget_message_text')]");
            var timeNode = messageNode.SelectSingleNode(".//a[contains(@class,'tgme_widget_message_date')]/time");

            if (textNode == null || timeNode == null)
                continue;

            var text = HtmlEntity.DeEntitize(textNode.InnerText.Trim());

            var dateStr = timeNode.GetAttributeValue("datetime", string.Empty);
            if (DateTimeOffset.TryParse(dateStr, null, DateTimeStyles.AssumeUniversal, out var timestamp))
            {
                if (timestamp.UtcDateTime.Date == DateTime.UtcNow.Date && !string.IsNullOrWhiteSpace(text))
                {
                    messages.Add(text);
                }
            }
        }

        return messages;
    }
}