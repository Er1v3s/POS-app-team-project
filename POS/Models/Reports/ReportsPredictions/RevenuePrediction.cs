using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.Reports.ReportsPredictions
{
    public class RevenuePrediction
    {
        public float[] ForecastedRevenue { get; set; }
        public float[] LowerBoundRevenue { get; set; }
        public float[] UpperBoundRevenue { get; set; }
    }
}
