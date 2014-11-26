using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ukrsklad.Domain.Utility
{
    public class PDVUtility
    {
        private PDVUtility()
        {
            PDV = 0;
        }
        private static PDVUtility instance = null;
        public static PDVUtility Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PDVUtility();
                }
                return instance;
            }
        }

        public double PDV { get; set; }

        public decimal getPriceWithPDV(decimal price)
        {
            return price + getPDV(price);
        }

        public decimal getPDV(decimal price)
        {
            return price * (decimal)PDV / 100;
        }
    }
}
