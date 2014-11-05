using ExportScanner.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace ExportScanner.Domain
{
    public class ScannerDB : IScannerDB, IDisposable
    {
        private OleDbConnection connection;

        public ScannerDB(string databaseLocation)
        {
            string connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", databaseLocation);
            connection = new OleDbConnection(connectionString);
            connection.Open();
        }
        
        public BillScanner GetBillForClient(int clientID)
        {
            BillScanner bill = new BillScanner();
            bill.ClientID = clientID;
            bill.ClientName = GetNameForClient(clientID);

            string sql = string.Format("select * from shipping where CLIENT_ID={0}", clientID);
            OleDbCommand command = new OleDbCommand(sql, connection);
            OleDbDataReader reader = command.ExecuteReader();
            DateTime? lastDateInBill = null;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int tovarId = reader.GetInt32(1);
                    float count = reader.GetFloat(5);
                    if (bill.Tovars.ContainsKey(tovarId))
                    {
                        bill.Tovars[tovarId] += count;
                    }
                    else 
                    {
                        bill.Tovars.Add(tovarId, count);
                    }

                    DateTime boxDate = reader.GetDateTime(2);

                    if (lastDateInBill == null)
                    {
                        lastDateInBill = boxDate;
                    }
                    else {
                        if (lastDateInBill.Value.CompareTo(boxDate) > 0) {
                            lastDateInBill = boxDate;
                        }
                    }
                }
            }
            bill.OutDate = lastDateInBill.Value;
            reader.Close();
            return bill;
        }

        public IEnumerable<int> GetDistinctClientsInBill()
        {
            List<int> distinctClients = new List<int>();
            string sql = @"select DISTINCT CLIENT_ID from shipping";
            OleDbCommand command = new OleDbCommand(sql, connection);
            OleDbDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    distinctClients.Add(reader.GetInt32(0));
                }
            }
            reader.Close();

            return distinctClients;
        }

        public string GetNameForClient(int clientID)
        {
            string name = null;
            string sql = string.Format("select * from clients where ID_CLIENT={0}", clientID);
            OleDbCommand command = new OleDbCommand(sql, connection);
            OleDbDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    name = reader.GetString(1);
                }
            }
            reader.Close();

            return name;
        }

        public void Dispose()
        {
            connection.Dispose();
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
