using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shadran.Ap.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shadran.AP.Exporter
{
    public class ExporterService : BackgroundService
    {
        private readonly ExporterOptions _options;
        private readonly TrackerMetricsService _metricsService;
        private readonly APTrackerReader _reader;
        private readonly Random _random;
        private const string TRACKER_ID = "GkC9ojFYQuW0lyY5k9fK6w";

        public ExporterService(IOptions<ExporterOptions> options, TrackerMetricsService metricsService, APTrackerReader reader)
        {
            _options = options.Value;
            _metricsService = metricsService;
            _reader = reader;
            _random = new Random();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                await UpdateData(_options.TrackerId);
                await Task.Delay(TimeSpan.FromSeconds(_options.PollingSeconds), stoppingToken);
            }
        }

        private async Task UpdateData(string trackerId)
        {
            var trackerDatas = await _reader.Read(trackerId);
            if (!trackerDatas.Any()) return;
            foreach (var d in trackerDatas)
            {
                _metricsService.SetTrackerData(d);
            }
        }
    }
}
