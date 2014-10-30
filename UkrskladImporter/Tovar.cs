using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ukrsklad.Domain.Model;

namespace UkrskladImporter
{
    public class Tovar
    {
        public string KOD { get; set; }
        public double Count { get; set; }
        public string Name { get; set; }
    }
}
