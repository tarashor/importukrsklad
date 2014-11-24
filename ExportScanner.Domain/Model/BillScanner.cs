using System;
using System.Collections.Generic;

namespace ExportScanner.Domain.Model
{
    [Serializable]
    public class BillScanner
    {
        private readonly SortedList<TovarScanner, double> tovars;
        public BillScanner() 
        {
            tovars = new SortedList<TovarScanner, double>(new TovarComparer());
        }

        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public SortedList<TovarScanner, double> Tovars { get { return tovars; } }
        public DateTime OutDate { get; set; }

        public override string ToString()
        {
            return ClientName + " " + OutDate.ToShortDateString();
        }

        [Serializable]
        private class TovarComparer : IComparer<TovarScanner>
        {
            public int Compare(TovarScanner x, TovarScanner y)
            {
                if (x.ID == y.ID) return 0;

                int groupComparator = x.GroupName.CompareTo(y.GroupName);
                if (groupComparator == 0)
                {
                    return x.Name.CompareTo(y.Name);
                }
                else
                {
                    return (-1)*groupComparator;
                }
            }
        }

    }

    
}
