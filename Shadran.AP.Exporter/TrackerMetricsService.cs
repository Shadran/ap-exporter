using Microsoft.Extensions.Logging;
using Shadran.AP.Exporter;
using System.Diagnostics.Metrics;

namespace Shadran.Ap.Services
{
    public class TrackerMetricsService
    {
        private readonly IMeterFactory _meterFactory;
        private readonly ILogger<TrackerMetricsService> _logger;
        private readonly Meter _meter;
        private ObservableGauge<int> _completedChecks;
        private ObservableGauge<int> _totalChecks;

        private readonly Dictionary<string, TrackerData> _trackerDatas = new Dictionary<string, TrackerData>();

        public TrackerMetricsService(IMeterFactory meterFactory, ILogger<TrackerMetricsService> logger)
        {
            _meterFactory = meterFactory;
            _logger = logger;

            _meter = _meterFactory.Create("shadran.ap.trackers", "1.0.0");
            IEnumerable<KeyValuePair<string, object?>> tags = [
                    new KeyValuePair<string, object?>("otel_scope_name", "shadran.ap"),
                    new KeyValuePair<string, object?>("otel_scope_version", "1.0.0.0"),
                    new KeyValuePair<string, object?>("type", "Shadran.Ap.Services.TrackerMetricsService")
                    ];
            _completedChecks = _meter.CreateObservableGauge("shadran.ap.trackers.completed_checks", GetTrackerData, "checks", "Number of completed checks", tags);
            _totalChecks = _meter.CreateObservableGauge("shadran.ap.trackers.total_checks", GetTotalData, "checks", "Number of total checks", tags);
        }

        public IEnumerable<Measurement<int>> GetTrackerData()
        {
            lock (_trackerDatas)
            {
                return _trackerDatas.Select(x => new Measurement<int>(x.Value.CurrentChecks, [
                    new KeyValuePair<string, object?>("player.name", x.Value.Player),
                    new KeyValuePair<string, object?>("player.total_checks", x.Value.TotalChecks),
                    new KeyValuePair<string, object?>("tracker.id", x.Value.TrackerId),
                ]));
            }
        }

        public IEnumerable<Measurement<int>> GetTotalData()
        {
            lock (_trackerDatas)
            {
                return _trackerDatas.Select(x => new Measurement<int>(x.Value.TotalChecks, [
                    new KeyValuePair<string, object?>("player.name", x.Value.Player),
                    new KeyValuePair<string, object?>("tracker.id", x.Value.TrackerId),
                ]));
            }
        }

        internal void SetTrackerData(TrackerData trackerData)
        {
            lock (_trackerDatas)
            {
                if (_trackerDatas.ContainsKey(trackerData.Player))
                {
                    _trackerDatas[trackerData.Player] = trackerData;
                }
                else
                {
                    _trackerDatas.TryAdd(trackerData.Player, trackerData);
                }
            }
        }
    }
}