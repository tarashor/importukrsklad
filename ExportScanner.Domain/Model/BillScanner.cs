using System;
using System.Collections.Generic;

namespace ExportScanner.Domain.Model
{
    [Serializable]
    public class BillScanner
    {
        private readonly Dictionary<int, double> tovars;
        public BillScanner() 
        {
            tovars = new Dictionary<int, double>();
        }

        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public IDictionary<int, double> Tovars { get { return tovars; } }
        public DateTime OutDate { get; set; }

        public override string ToString()
        {
            return ClientName + " " + OutDate.ToShortDateString();
        }
    }
}
