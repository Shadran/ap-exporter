using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shadran.AP.Exporter
{
    public class APTrackerReader(HttpClient httpClient, ILogger<APTrackerReader> logger)
    {
        private const int NAME_INDEX = 1;
        private const int CHECKS_INDEX = 4;
        private readonly Regex _checksRegex = new(@"(\d+)/(\d+)");
        public async Task<IEnumerable<TrackerData>> Read(string trackerId)
        {
            List<TrackerData> results = new();
            try
            {
                var page = await httpClient.GetStringAsync($"https://archipelago.gg/tracker/{trackerId}");
                var doc = new HtmlDocument();
                doc.LoadHtml(page);


                var tableRows = doc.DocumentNode.SelectSingleNode("//table[@id='checks-table']")?.SelectSingleNode(".//tbody")?.SelectNodes(".//tr");
                if (tableRows == null) return results;
                foreach (var row in tableRows)
                {
                    var cells = row.SelectNodes(".//td");
                        var name = cells[NAME_INDEX].InnerText;
                        var checksMatch = _checksRegex.Match(cells[CHECKS_INDEX].InnerText);
                        var checksNumber = int.Parse(checksMatch.Groups[1].Value);
                        var checksTotal = int.Parse(checksMatch.Groups[2].Value);
                        results.Add(new TrackerData(name, checksNumber, checksTotal, trackerId));
                    }
                }
            catch (Exception ex)
            {
                logger.LogError($"Error while parsing HTML data: {ex}");
                return [];
            }
            return results;
        }
    }
}
