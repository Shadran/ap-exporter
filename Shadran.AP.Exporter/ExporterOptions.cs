using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shadran.AP.Exporter
{
    public class ExporterOptions
    {
        public string TrackerId { get; set; }
        public int PollingSeconds { get; set; }
        public ArchipelagoFilters Filters { get; set; } = new();
    }

    public class ArchipelagoFilters
    {
        public string[] Games { get; set; } = [];
        public string[] Players { get; set; } = [];
    }
}
