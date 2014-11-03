using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportScanner.Domain.Model
{
    [Serializable]
    public class BillScanner
    {
        public Dictionary<int, double> tovars;
        public BillScanner() 
        {
            tovars = new Dictionary<int, double>();
        }

        public int ClientID { get; set; }
        public IDictionary<int, double> Tovars { get { return tovars; } } 
    }
}
