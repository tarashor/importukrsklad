using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ukrsklad.Domain.Utility
{
    public interface ILogger
    {
        void Log(string log);
        string FileName { get; set; }
        void StartLogging();
        void StopLogging();
    }
}
