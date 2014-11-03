using ExportScanner.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace ExportScanner.Domain
{
    public class ScannerDB : IScannerDB
    {
        private OleDbConnection connection;

        public ScannerDB(string databaseLocation)
        {
            /*string sCurrentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            CultureInfo ci = new CultureInfo(sCurrentCulture);
            ci.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            */
            //C:\Shipping\BC_Tables.mdb
            string connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", databaseLocation);
            connection = new OleDbConnection(connectionString);
            connection.Open();

        }

        

        
        public BillScanner GetBillForClient(int clientID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetDistinctClientsInBill()
        {
            List<int> distinctClients = new List<int>();

            return distinctClients;
        }

        public string GetNameForClient(int clientID)
        {
            throw new NotImplementedException();
        }
    }
}
