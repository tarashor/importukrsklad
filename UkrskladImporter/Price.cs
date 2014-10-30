using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ukrsklad.Domain.Model;

namespace UkrskladImporter
{
    public class Price
    {
        public PriceType Value { get; set; }
        public string Name { get; set; }
    }
}
