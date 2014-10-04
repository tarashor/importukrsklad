using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportUkrSklad
{
    class Program
    {
        /*string sql = "INSERT INTO SALES (PO_NUMBER, CUST_NO, SALES_REP, ORDER_STATUS, " +
            "ORDER_DATE, SHIP_DATE, DATE_NEEDED, PAID, QTY_ORDERED, TOTAL_VALUE, " +
            "DISCOUNT, ITEM_TYPE) VALUES (@po_number, 1004, 11, 'new', " +
            "'1991-03-04 00:00:00', '1991-03-05 00:00:00', NULL, 'y', 10, 5000, " +
            "0.100000001490116, 'hardware');";

         * command.Parameters.Add("@po_number", FbDbType.Char, 8);
         * 
            for (int i = 360; i < 365; i++)
            {
                command.Parameters[0].Value = "V91E0" + i.ToString();
                command.ExecuteNonQuery();

            }

            System.Threading.Thread.Sleep(2000);*/

        static void Main(string[] args)
        {
            
            FbConnectionStringBuilder cs = new FbConnectionStringBuilder();
            cs.Database = @"c:\ProgramData\UkrSklad\db\Sklad.tcb";
            cs.UserID = "SYSDBA";
            cs.Password = "masterkey";
            cs.Charset = "NONE";
            cs.Pooling = false;
            cs.ServerType = FbServerType.Embedded;

            FbConnection connection = new FbConnection(cs.ToString());
            try
            {
                connection.Open();

                string sql = "select * from TOVAR_NAME"; 

                FbCommand command = new FbCommand(sql, connection);

                FbDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("{0}\t{1}", reader.GetInt32(0),
                            reader.GetString(1));
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
