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

        //string sql = "select * from TOVAR_NAME"; 
        /*FbDataReader reader = command.ExecuteReader();
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
                reader.Close();*/

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

                createBills(connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
                Console.ReadLine();
            }


        }

        private static void createBills(FbConnection connection)
        {
            for (int i = 4; i < 100; i++)
            {
                string sql = createNakladnaSQL(i);
                FbCommand command = new FbCommand(sql, connection);
                command.ExecuteNonQuery();
                Console.WriteLine("Generated {0}", i);
            }
        }


        private const string dateFormat = "yyyy-mm-dd hh:mm:ss";

        private const string insertVNAKL = @"INSERT INTO VNAKL (NUM,    FIRMA_ID,   NU,     DATE_DOK,    CLIENT,             CENA,       CENA_PDV,       CENA_ZNIG,      DOV_SER,        DOV_NUM,        DOV_DATA,       DOV_FIO,        CLIENT_ID,      PDV,        PROD_UMOV,      ZNIG_TYPE,      POD_REKL_PER,       POD_REKL_CH,   FIO_BUH,    NU_ID,      SKLAD_ID,   CURR_TYPE,      CURR_CENA,      CURR_CENA_PDV,      CURR_CENA_ZNIG,     CURR_PDV,       CURR_POD_REKL_CH,   CLIENT_RAH_ID,  AFIRM_RAH_ID,       DOPOLN,         CENA_TOV_TRANS, CURR_CENA_TOV_TRANS,    IS_MOVE,    DOC_MARK_TYPE,      DOC_USER_ID,        DOC_CREATE_TIME,    DOC_MODIFY_TIME,    DOC_DESCR,      ARTICLE_ID, TTN_NU, TTN_DATE_DOK, TTN_DOR_LIST, TTN_AVTO, TTN_AVTO_PIDPR, TTN_VODITEL, TTN_ADR_FROM, TTN_ADR_TO, IM_NUM, ZNIG_PROC, TTN_AVTO_PRIC, TTN_VID_PEREV, TTN_KOLVO_MIS, TTN_MAS_BRUT, PDV_TYPE)
                                                        VALUES (NULL,   {0},        '{1}',  '{2}',       '{3}',              {4},        {5},            0,              '',             '',             NULL,           '',             {6},            {7},        '',             0,              NULL,               0,             NULL,       {8},        {9},        0,              {10},           {11},               0,                  {12},           0,                  0,              0,                  '',             0, 0,                                   1,          0,                  {13},               '{14}',             '{15}',             '',             0, '', '1899-12-30 00:00:00', '', '', '', '', '', '', -1, 0, '', '', '', '', 0);";

        private static string createNakladnaSQL(int firmaID, int billNumber, DateTime billDate, string client, decimal price, decimal totalPrice, int clientID, decimal pdv, int nuID, int skladID, decimal currentPrice, decimal currentTotalPrice, decimal currentPdv, int userID, DateTime billCreated, DateTime billModified)
        {
            return string.Format(insertVNAKL, firmaID, billNumber, billDate, client, price, totalPrice, clientID, pdv, nuID, skladID, currentPrice, currentTotalPrice, currentPdv, userID, billCreated, billModified);
        }


    }
}
