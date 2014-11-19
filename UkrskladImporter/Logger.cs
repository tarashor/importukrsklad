using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ukrsklad.Domain.Utility;

namespace UkrskladImporter
{
    public class Logger:ILogger
    {
        public string strLogFilePath = string.Empty;
        private StreamWriter sw = null;
        
        public Logger(string filename) {
            sw = new StreamWriter(filename);
        }
        public void Log(string log)
        {
            if (sw != null)
                sw.WriteLine(log);
        }

        public void Close()
        {
            sw.Close();
        }
    }
}
