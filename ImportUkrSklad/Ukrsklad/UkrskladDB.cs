using FirebirdSql.Data.FirebirdClient;
using ImportUkrSklad.Input;
using ImportUkrSklad.Ukrsklad.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportUkrSklad.Ukrsklad
{
    internal class UkrskladDB
    {
        private const string insertVNAKL = @"INSERT INTO VNAKL (NUM,    FIRMA_ID,   NU,     DATE_DOK,    CLIENT,             CENA,       CENA_PDV,       CENA_ZNIG,      DOV_SER,        DOV_NUM,        DOV_DATA,       DOV_FIO,        CLIENT_ID,      PDV,        PROD_UMOV,      ZNIG_TYPE,      POD_REKL_PER,       POD_REKL_CH,   FIO_BUH,    NU_ID,      SKLAD_ID,   CURR_TYPE,      CURR_CENA,      CURR_CENA_PDV,      CURR_CENA_ZNIG,     CURR_PDV,       CURR_POD_REKL_CH,   CLIENT_RAH_ID,  AFIRM_RAH_ID,       DOPOLN,         CENA_TOV_TRANS, CURR_CENA_TOV_TRANS,    IS_MOVE,    DOC_MARK_TYPE,      DOC_USER_ID,        DOC_CREATE_TIME,    DOC_MODIFY_TIME,    DOC_DESCR,      ARTICLE_ID, TTN_NU, TTN_DATE_DOK, TTN_DOR_LIST, TTN_AVTO, TTN_AVTO_PIDPR, TTN_VODITEL, TTN_ADR_FROM, TTN_ADR_TO, IM_NUM, ZNIG_PROC, TTN_AVTO_PRIC, TTN_VID_PEREV, TTN_KOLVO_MIS, TTN_MAS_BRUT, PDV_TYPE)
                                                        VALUES (NULL,   {0},        '{1}',  '{2}',       '{3}',              {4},        {5},            0,              '',             '',             NULL,           '',             {6},            {7},        '',             0,              NULL,               0,             NULL,       {8},        {9},        0,              {10},           {11},               0,                  {12},           0,                  0,              0,                  '',             0, 0,                                   1,          0,                  {13},               '{14}',             '{15}',             '',             0, '', '1899-12-30 00:00:00', '', '', '', '', '', '', -1, 0, '', '', '', '', 0);";

        private const string insertVNAKL_ = @"INSERT INTO VNAKL_ (NUM,  PID,    TOV_NAME,   TOV_KOLVO,  TOV_CENA,   TOV_ED,     TOV_CENA_PDV,   TOV_KOLVO_PART, TOVAR_ID,       CURR_TOV_CENA,      CURR_TOV_CENA_PDV,      SKLAD_ID,       TOV_SUMA,   CURR_TOV_SUMA, DOC_DOPOLN, IS_PDV, COMPL_ID, IS_COMPL, COMPL_KOLVO_DEF, COMPL_IS_CONST, TOV_SUMA_ZNIG, CURR_TOV_SUMA_ZNIG)
                                                          VALUES (NULL, {0},    '{1}',      {2},        {3},        '{4}',      {5},            1,              {6},            {7},                {8},                    {9},            {10},       {11}, '', -1, 0, 0, 0, 0, 0, 0);";

        private const string insertTOVAR_MOVE = @"INSERT INTO TOVAR_MOVE (NUM,      DOC_TYPE_ID,    DOC_ID,     MDATE,      TOVAR_ID,   FROM_FIRMA_ID,      FROM_SKLAD_ID,      FROM_KOLVO,     FROM_CENA,      FROM_SUMA,      TO_FIRMA_ID,        TO_SKLAD_ID,        TO_KOLVO,       TO_CENA,        TO_SUMA,        CURR_TYPE,      CURR_FROM_CENA,         CURR_FROM_SUMA,         CURR_TO_CENA,   CURR_TO_SUMA,   FROM_FIRMA_RAH_ID, TO_FIRMA_RAH_ID, CENA_ZNIG_DIFF, SUMA_ZNIG_DIFF, CURR_CENA_ZNIG_DIFF, CURR_SUMA_ZNIG_DIFF,       CENA_PDV, SUMA_PDV, CURR_CENA_PDV, CURR_SUMA_PDV,       ARTICLE_ID, COMPL_ID, IS_USLUGA)
                                                                  VALUES (NULL,     1,              {0},        '{1}',      {2},        {3},                {4},                {5},            {6},            {7},            {8},                {9},                {10},           {11},           {12},           0,              {13},                   {14},                   {15},           {16},           0, 0, 0, 0, 0, 0,                                                                                                   {17}, {18}, {19}, {20},                                 0, 0, 0);";

        private const string updateBillPrice = @"UPDATE VNAKL SET CENA = {0}, CENA_PDV = {1}, PDV = {2}, CURR_CENA = {3}, CURR_CENA_PDV = {4}, CURR_PDV = {5} WHERE (NUM = {6});";

        private const string dateFormat = "yyyy-MM-dd hh:mm:ss";


        public static void createBill(FbConnection connection, int billNumber, int fromClientID, int toClientID, PriceType priceType, int skladID, List<Tovar> tovars)
        {
            createBillHeader(connection, billNumber, skladID, fromClientID, toClientID);
            int billID = getLastBillID(connection);
            decimal total = 0;
            foreach (Tovar tovar in tovars)
            {
                total += addTovarToBill(connection, billID, tovar.TovarKOD, tovar.Count, priceType, skladID, fromClientID, toClientID);
            }

            setTotal(connection, billID, total);
        }

        private static void setTotal(FbConnection connection, int billID, decimal total)
        {
            string updateSQL = updateBillPriceSQL(billID, total);
            FbCommand command = new FbCommand(updateSQL, connection);
            command.ExecuteNonQuery();
        }

        private static decimal addTovarToBill(FbConnection connection, int billID, string tovarID, double count, PriceType priceType, int skladID, int fromClientID, int toClientID)
        {
            UkrSkladTovar tovar = getTovarByKOD(connection, tovarID);
            decimal price = getPrice(tovar, priceType);
            decimal pricePDV = PDVUtility.getPriceWithPDV(price);
            decimal sum = (decimal)count * price;

            string billContentSQL = createBillContentSQL(billID, tovar.Name, tovar.ID, tovar.MeasurementUnits, count, price, pricePDV, price, pricePDV, skladID, sum, sum);
            FbCommand command = new FbCommand(billContentSQL, connection);
            command.ExecuteNonQuery();

            createTovarMove(connection, billID, tovar.ID, fromClientID, skladID, count, price, toClientID);
            //update

            return sum;
        }

        private static void createTovarMove(FbConnection connection, int billID, int tovarID, int fromFirmaID, int fromSkladID, double count, decimal price, int toFirmaID)
        {
            decimal sum = (decimal)count * price;
            decimal pricePDV = PDVUtility.getPDV(price);
            decimal sumPDV = PDVUtility.getPDV(sum);
            int toSkladId = 0; //because it is out 
            string insertTovarMove = createTovarMoveSQL(billID, DateTime.Today, tovarID, fromFirmaID, fromSkladID, count, price, sum, toFirmaID, toSkladId, count, price, sum, price, sum, price, sum, pricePDV, sumPDV, pricePDV, sumPDV);
            FbCommand command = new FbCommand(insertTovarMove, connection);
            command.ExecuteNonQuery();
        }

        private static void createBillHeader(FbConnection connection, int billNumber, int skladID, int fromClientID, int toClientID)
        {
            int userID = 1;

            string clientName = getClientNameByID(connection, toClientID);
            string billSQL = createBillSQL(fromClientID, billNumber, DateTime.Today, clientName, 0, 0, toClientID, 0, billNumber, skladID, 0, 0, 0, userID, DateTime.Now, DateTime.Now);
            FbCommand command = new FbCommand(billSQL, connection);
            command.ExecuteNonQuery();
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
            string sql = "select * from CLIENT where NUM=" + clientID.ToString();
            FbCommand command = new FbCommand(sql, connection);
            FbDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetName(i) == "FIO")
                            {
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

        

        
        private static string createBillSQL(int firmaID, int billNumber, DateTime billDate, string client, decimal price, decimal totalPrice, int clientID, decimal pdv, int nuID, int skladID, decimal currentPrice, decimal currentTotalPrice, decimal currentPdv, int userID, DateTime billCreated, DateTime billModified)
        {
            return string.Format(insertVNAKL, firmaID, billNumber, billDate.ToString(dateFormat), client, price, totalPrice, clientID, pdv, nuID, skladID, currentPrice, currentTotalPrice, currentPdv, userID, billCreated.ToString(dateFormat), billModified.ToString(dateFormat));
        }

        private static string createBillContentSQL(int billID, string tovarName, int tovarID, string odVymiru, double count, decimal price, decimal totalPrice, decimal currentPrice, decimal currentTotalPrice, int skladID, decimal sumPrice, decimal currentSumPrice)
        {
            return string.Format(insertVNAKL_, billID, tovarName, count, price, odVymiru, totalPrice, tovarID, currentPrice, currentTotalPrice, skladID, sumPrice, currentSumPrice);
        }

        private static string createTovarMoveSQL(int billID, DateTime date, int tovarID, int FromFirmaID, int FromSkladID, double fromCount, decimal fromPrice, decimal fromSum, int toFirmaID, int toSkladID, double toCount, decimal toPrice, decimal toSum, decimal currentFromPrice, decimal currentFromSum, decimal currentToPrice, decimal currentToSum, decimal pricePDV, decimal sumPDV,decimal currentPricePDV, decimal currentSumPDV)
        {
            return string.Format(insertTOVAR_MOVE, billID, date.ToString(dateFormat), tovarID, FromFirmaID, FromSkladID, fromCount, fromPrice, fromSum, toFirmaID, toSkladID, toCount, toPrice, toSum, currentFromPrice, currentFromSum, currentToPrice, currentToSum, pricePDV, sumPDV, currentPricePDV, currentSumPDV);
        }

        private static string updateBillPriceSQL(int billID, decimal price)
        {
            return string.Format(updateBillPrice, price, PDVUtility.getPriceWithPDV(price), PDVUtility.getPDV(price), price, PDVUtility.getPriceWithPDV(price), PDVUtility.getPDV(price), billID);
        }

        

        

        private static decimal getPrice(UkrSkladTovar tovar, PriceType priceType)
        {
            decimal result = 0;
            switch (priceType)
            {
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
