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
        private StreamWriter sw = null;
        private string filename = null;
        
        public Logger() {
            
        }
        public void Log(string log)
        {
            if (sw != null)
                sw.WriteLine(log);
        }

        public void StartLogging()
        {
            if (filename != null)
            {
                sw = new StreamWriter(filename);
            }
        }

        public void StopLogging()
        {
            sw.Close();
        }


        public string FileName
        {
            get
            {
                return filename;
            }
            set
            {
                filename = value;
            }
        }
    }
}
