using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.ViewModels.ReportsAndAnalysis.Interfaces
{
    interface IChartGenerator<T>
    {
        void GenerateChar(List<T> data);
    }
}
