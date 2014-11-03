using ExportScanner.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportScanner.Domain
{
    public interface IScannerDB
    {
        BillScanner GetBillForClient(int clientID);
        IEnumerable<int> GetDistinctClientsInBill();
        string GetNameForClient(int clientID);
    }
}
