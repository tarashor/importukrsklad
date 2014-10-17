using FirebirdSql.Data.FirebirdClient;
using ImportUkrSklad.Input;
using ImportUkrSklad.Ukrsklad;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImportUkrSklad
{
    class Program
    {
        static void Main(string[] args)
        {
            //Get current culture 
            string sCurrentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            CultureInfo ci = new CultureInfo(sCurrentCulture);
            ci.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;

            

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
                List<Tovar> tovars = new List<Tovar>();
                tovars.Add(new Tovar() { Count = 11.2, TovarKOD = "111" });
                tovars.Add(new Tovar() { Count = 5, TovarKOD = "111" });
                tovars.Add(new Tovar() { Count = 1, TovarKOD = "111" });
                int billNumber = 12;
                int fromClientID = 1;
                int toClintID = 2;
                int skladID = 1;
                UkrskladDB.createBill(connection, billNumber, fromClientID, toClintID, PriceType.CinaOptova, skladID, tovars);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
                Console.Write("--------------------Finished!-----------------");
                Console.ReadLine();
            }
        }

        

        


    }
}
