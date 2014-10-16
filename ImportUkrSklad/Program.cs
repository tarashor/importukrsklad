using FirebirdSql.Data.FirebirdClient;
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
        private const double PDV = 20; 

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
                createBill(connection, 10, 3, PriceType.CinaOptova, 1, tovars);

                //createBill(connection);
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

        private static void createBill(FbConnection connection, int billNumber, int clientID, PriceType priceType, int skladID, List<Tovar> tovars)
        {
            createBillHeader(connection, billNumber, clientID);
            int billID = getLastBillID(connection);
            decimal total = 0;
            foreach (Tovar tovar in tovars) {
                total += addTovarToBill(connection, billID, tovar.TovarKOD, tovar.Count, priceType, skladID);
            }

            setTotal(connection, billID, total);
        }

        private static void setTotal(FbConnection connection, int billID, decimal total)
        {
            string updateSQL = updateBillPriceSQL(billID, total);
            FbCommand command = new FbCommand(updateSQL, connection);
            command.ExecuteNonQuery();
        }

        private static decimal addTovarToBill(FbConnection connection, int billID, string tovarID, double count, PriceType priceType, int skladID)
        {
            UkrSkladTovar tovar = getTovarByKOD(connection, tovarID);
            decimal price = getPrice(tovar, priceType);
            decimal pricePDV = getPriceWithPDV(price);
            decimal sum = (decimal)count * price;

            string billContentSQL = createBillContentSQL(billID, tovar.Name, tovar.ID, tovar.MeasurementUnits, count, price, pricePDV, price, pricePDV, skladID, sum, sum);
            FbCommand command = new FbCommand(billContentSQL, connection);
            command.ExecuteNonQuery();
            return sum;
        }

        private static void createBillHeader(FbConnection connection, int billNumber, int clientID)
        {
            int skladID = 1;
            int userID = 1;

            string clientName = getClientNameByID(connection, clientID);
            string billSQL = createBillSQL(1, billNumber, DateTime.Now, clientName, 0, 0, clientID, 0, billNumber, skladID, 0, 0, 0, userID, DateTime.Now, DateTime.Now);
            FbCommand command = new FbCommand(billSQL, connection);
            command.ExecuteNonQuery();
        }

        private const string dateFormat = "yyyy-MM-dd hh:mm:ss";

        private const string insertVNAKL = @"INSERT INTO VNAKL (NUM,    FIRMA_ID,   NU,     DATE_DOK,    CLIENT,             CENA,       CENA_PDV,       CENA_ZNIG,      DOV_SER,        DOV_NUM,        DOV_DATA,       DOV_FIO,        CLIENT_ID,      PDV,        PROD_UMOV,      ZNIG_TYPE,      POD_REKL_PER,       POD_REKL_CH,   FIO_BUH,    NU_ID,      SKLAD_ID,   CURR_TYPE,      CURR_CENA,      CURR_CENA_PDV,      CURR_CENA_ZNIG,     CURR_PDV,       CURR_POD_REKL_CH,   CLIENT_RAH_ID,  AFIRM_RAH_ID,       DOPOLN,         CENA_TOV_TRANS, CURR_CENA_TOV_TRANS,    IS_MOVE,    DOC_MARK_TYPE,      DOC_USER_ID,        DOC_CREATE_TIME,    DOC_MODIFY_TIME,    DOC_DESCR,      ARTICLE_ID, TTN_NU, TTN_DATE_DOK, TTN_DOR_LIST, TTN_AVTO, TTN_AVTO_PIDPR, TTN_VODITEL, TTN_ADR_FROM, TTN_ADR_TO, IM_NUM, ZNIG_PROC, TTN_AVTO_PRIC, TTN_VID_PEREV, TTN_KOLVO_MIS, TTN_MAS_BRUT, PDV_TYPE)
                                                        VALUES (NULL,   {0},        '{1}',  '{2}',       '{3}',              {4},        {5},            0,              '',             '',             NULL,           '',             {6},            {7},        '',             0,              NULL,               0,             NULL,       {8},        {9},        0,              {10},           {11},               0,                  {12},           0,                  0,              0,                  '',             0, 0,                                   1,          0,                  {13},               '{14}',             '{15}',             '',             0, '', '1899-12-30 00:00:00', '', '', '', '', '', '', -1, 0, '', '', '', '', 0);";

        private const string insertVNAKL_ = @"INSERT INTO VNAKL_ (NUM,  PID,    TOV_NAME,   TOV_KOLVO,  TOV_CENA,   TOV_ED,     TOV_CENA_PDV,   TOV_KOLVO_PART, TOVAR_ID,       CURR_TOV_CENA,      CURR_TOV_CENA_PDV,      SKLAD_ID,       TOV_SUMA,   CURR_TOV_SUMA, DOC_DOPOLN, IS_PDV, COMPL_ID, IS_COMPL, COMPL_KOLVO_DEF, COMPL_IS_CONST, TOV_SUMA_ZNIG, CURR_TOV_SUMA_ZNIG)
                                                          VALUES (NULL, {0},    '{1}',      {2},        {3},        '{4}',      {5},            1,              {6},            {7},                {8},                    {9},            {10},       {11}, '', -1, 0, 0, 0, 0, 0, 0);";

        private const string updateBillPrice = @"UPDATE VNAKL SET CENA = {0}, CENA_PDV = {1}, PDV = {2}, CURR_CENA = {3}, CURR_CENA_PDV = {4}, CURR_PDV = {5} WHERE (NUM = {6});";

        private static string createBillSQL(int firmaID, int billNumber, DateTime billDate, string client, decimal price, decimal totalPrice, int clientID, decimal pdv, int nuID, int skladID, decimal currentPrice, decimal currentTotalPrice, decimal currentPdv, int userID, DateTime billCreated, DateTime billModified)
        {
            return string.Format(insertVNAKL, firmaID, billNumber, billDate.ToString(dateFormat), client, price, totalPrice, clientID, pdv, nuID, skladID, currentPrice, currentTotalPrice, currentPdv, userID, billCreated.ToString(dateFormat), billModified.ToString(dateFormat));
        }

        private static string createBillContentSQL(int billID, string tovarName, int tovarID, string odVymiru, double count, decimal price, decimal totalPrice, decimal currentPrice, decimal currentTotalPrice, int skladID, decimal sumPrice, decimal currentSumPrice)
        {
            return string.Format(insertVNAKL_, billID, tovarName, count, price, odVymiru, totalPrice, tovarID, currentPrice, currentTotalPrice, skladID, sumPrice, currentSumPrice);
        }

        private static string updateBillPriceSQL(int billID, decimal price) {
            return string.Format(updateBillPrice, price, getPriceWithPDV(price), getPDV(price), price, getPriceWithPDV(price), getPDV(price), billID);
        }

        

        private static int getLastBillID(FbConnection connection)
        {
            int lastBillID = -1;
            string sql = "select * from VNAKL ORDER BY NUM DESC";
            FbCommand command = new FbCommand(sql, connection);
            FbDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        lastBillID = reader.GetInt32(0);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                reader.Close();
            }

            return lastBillID;
        }

        private static string getClientNameByID(FbConnection connection, int clientID)
        {
            string clientName = string.Empty;
            string sql = "select * from CLIENT where NUM="+clientID.ToString();
            FbCommand command = new FbCommand(sql, connection);
            FbDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++) {
                            if (reader.GetName(i) == "FIO") {
                                clientName = reader.GetString(i);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                reader.Close();
            }

            return clientName;
        }

        private static UkrSkladTovar getTovarByKOD(FbConnection connection, string tovarKOD)
        {
            UkrSkladTovar tovar = null;
            string sql = string.Format("select * from TOVAR_NAME where KOD='{0}'", tovarKOD);
            FbCommand command = new FbCommand(sql, connection);
            FbDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        tovar = new UkrSkladTovar();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string column = reader.GetName(i);
                            if (column == "NUM")
                            {
                                tovar.ID = reader.GetInt32(i);
                            }
                            if (column == "NAME")
                            {
                                tovar.Name = reader.GetString(i);
                            }
                            if (column == "ED_IZM")
                            {
                                tovar.MeasurementUnits = reader.GetString(i);
                            }
                            if (column == "CENA")
                            {
                                tovar.Cina = reader.GetDecimal(i);
                            }
                            if (column == "CENA_O")
                            {
                                tovar.CinaOptova = reader.GetDecimal(i);
                            }
                            if (column == "CENA_R")
                            {
                                tovar.CinaRozdrib = reader.GetDecimal(i);
                            }

                            if (column == "CENA_1")
                            {
                                tovar.Cina1 = reader.GetDecimal(i);
                            }
                            if (column == "CENA_2")
                            {
                                tovar.Cina2 = reader.GetDecimal(i);
                            }
                            if (column == "CENA_3")
                            {
                                tovar.Cina3 = reader.GetDecimal(i);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                reader.Close();
            }

            return tovar;
        }

        private static decimal getPriceWithPDV(decimal price)
        {
            return price + getPDV(price);
        }

        private static decimal getPDV(decimal price)
        {
            return price * (decimal)PDV / 100;
        }

        private static decimal getPrice(UkrSkladTovar tovar, PriceType priceType)
        {
            decimal result = 0;
            switch (priceType) {
                case PriceType.Cina: result = tovar.Cina; break;
                case PriceType.Cina1: result = tovar.Cina1; break;
                case PriceType.Cina2: result = tovar.Cina2; break;
                case PriceType.Cina3: result = tovar.Cina3; break;
                case PriceType.CinaOptova: result = tovar.CinaOptova; break;
                case PriceType.CinaRozdrib: result = tovar.CinaRozdrib; break;
            }
            
            return result;
        }

        


    }
}
