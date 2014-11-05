using ExportScanner.Domain.Model;
using System.Collections.Generic;

namespace ExportScanner.Domain
{
    public interface IScannerDB
    {
        BillScanner GetBillForClient(int clientID);
        IEnumerable<int> GetDistinctClientsInBill();
        string GetNameForClient(int clientID);
        void Close();
    }
}
