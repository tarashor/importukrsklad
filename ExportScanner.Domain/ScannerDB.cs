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
            List<TovarScanner> tovars = loadTovars();

            string sql = string.Format("select * from shipping where CLIENT_ID={0}", clientID);
            OleDbCommand command = new OleDbCommand(sql, connection);
            OleDbDataReader reader = command.ExecuteReader();
            DateTime? lastDateInBill = null;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int tovarId = reader.GetInt32(1);
                    TovarScanner tovar = tovars.Find(t => t.ID == tovarId);
                    float count = reader.GetFloat(5);

                    if (bill.Tovars.ContainsKey(tovar))
                    {
                        bill.Tovars[tovar] += count;
                    }
                    else 
                    {
                        bill.Tovars.Add(tovar, count);
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

        public List<TovarScanner> loadTovars()
        {
            List<TovarScanner> tovars = new List<TovarScanner>();
            string sql = "SELECT GOODS.NAME, GROUPS.GROUP_NAME, GOODS.ID_GOODS FROM GROUPS INNER JOIN GOODS ON GROUPS.ID_GROUP = GOODS.GROUP_ID ORDER BY GOODS.NAME, GROUPS.GROUP_NAME";
            OleDbCommand command = new OleDbCommand(sql, connection);
            OleDbDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    TovarScanner tovar = new TovarScanner();
                    tovar.ID = reader.GetInt32(2);
                    tovar.Name = reader.GetString(0);
                    tovar.GroupName = reader.GetString(1);
                    tovars.Add(tovar);
                }
            }
            reader.Close();

            return tovars;
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
