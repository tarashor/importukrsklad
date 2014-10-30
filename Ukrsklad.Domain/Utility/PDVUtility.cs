using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ukrsklad.Domain.Utility
{
    public class PDVUtility
    {
        private const double PDV = 20;

        public static decimal getPriceWithPDV(decimal price)
        {
            return price + getPDV(price);
        }

        public static decimal getPDV(decimal price)
        {
            return price * (decimal)PDV / 100;
        }
    }
}
