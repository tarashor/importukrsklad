using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ukrsklad.Domain.Model;

namespace UkrskladImporter
{
    public class Bill
    {
        private List<Tovar> tovars;
        public Bill() {
            tovars = new List<Tovar>();
        }
        public IList<Tovar> Tovars { get { return tovars; } }
        public Client FromClient { get; set; }
        public Client ToClient { get; set; }
        public Sklad Sklad { get; set; }
    }
}
