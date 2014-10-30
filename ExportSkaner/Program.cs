using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportSkaner
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Shipping\BC_Tables.mdb";
            OleDbConnection connection = new OleDbConnection(connectionString);
            try {
                connection.Open();
                string sql = @"select * from shipping";
                OleDbCommand command = new OleDbCommand(sql, connection);
                OleDbDataReader reader = command.ExecuteReader();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write(reader.GetName(i) + "\t");
                }

                Console.WriteLine();

                Console.WriteLine("======================================");

                int rowIndex = 0;

                if (reader.HasRows){
                    while (reader.Read() && (rowIndex < 10)) {
                        for (int i = 0; i < reader.FieldCount; i++) {
                            Console.Write(reader.GetValue(i) + "\t");
                        }
                        rowIndex++;
                        Console.WriteLine();

                    }
                }
                reader.Close();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            finally {
                connection.Close();
            }
        }
    }
}
