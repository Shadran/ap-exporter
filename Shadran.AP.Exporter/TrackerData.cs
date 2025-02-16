using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shadran.AP.Exporter
{
    public record TrackerData(string Player, int CurrentChecks, int TotalChecks, string TrackerId, string Game)
    {
    }
}
