using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ukrsklad.Domain.Model;
using Ukrsklad.Domain.Utility;

namespace Ukrsklad.Domain
{
    public class UkrskladDB : IUkrskladDB, IDisposable
    {
        private FbConnection connection;

        public UkrskladDB(string databaseLocation) {
            string sCurrentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            CultureInfo ci = new CultureInfo(sCurrentCulture);
            ci.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;

            FbConnectionStringBuilder cs = new FbConnectionStringBuilder();
            cs.Database = databaseLocation;
            cs.UserID = "SYSDBA";
            cs.Password = "masterkey";
            cs.Charset = "NONE";
            cs.Pooling = false;
            cs.ServerType = FbServerType.Embedded;

            connection = new FbConnection(cs.ToString());
            connection.Open();

        }

        private const string insertVNAKL = @"INSERT INTO VNAKL (NUM,    FIRMA_ID,   NU,     DATE_DOK,    CLIENT,             CENA,       CENA_PDV,       CENA_ZNIG,      DOV_SER,        DOV_NUM,        DOV_DATA,       DOV_FIO,        CLIENT_ID,      PDV,        PROD_UMOV,      ZNIG_TYPE,      POD_REKL_PER,       POD_REKL_CH,   FIO_BUH,    NU_ID,      SKLAD_ID,   CURR_TYPE,      CURR_CENA,      CURR_CENA_PDV,      CURR_CENA_ZNIG,     CURR_PDV,       CURR_POD_REKL_CH,   CLIENT_RAH_ID,  AFIRM_RAH_ID,       DOPOLN,         CENA_TOV_TRANS, CURR_CENA_TOV_TRANS,    IS_MOVE,    DOC_MARK_TYPE,      DOC_USER_ID,        DOC_CREATE_TIME,    DOC_MODIFY_TIME,    DOC_DESCR,      ARTICLE_ID, TTN_NU, TTN_DATE_DOK, TTN_DOR_LIST, TTN_AVTO, TTN_AVTO_PIDPR, TTN_VODITEL, TTN_ADR_FROM, TTN_ADR_TO, IM_NUM, ZNIG_PROC, TTN_AVTO_PRIC, TTN_VID_PEREV, TTN_KOLVO_MIS, TTN_MAS_BRUT, PDV_TYPE)
                                                        VALUES (NULL,   {0},        '{1}',  '{2}',       '{3}',              {4},        {5},            0,              '',             '',             NULL,           '',             {6},            {7},        '',             0,              NULL,               0,             NULL,       {8},        {9},        0,              {10},           {11},               0,                  {12},           0,                  0,              0,                  '',             0, 0,                                   1,          0,                  {13},               '{14}',             '{15}',             '',             0, '', '1899-12-30 00:00:00', '', '', '', '', '', '', -1, 0, '', '', '', '', 0);";

        private const string insertVNAKL_ = @"INSERT INTO VNAKL_ (NUM,  PID,    TOV_NAME,   TOV_KOLVO,  TOV_CENA,   TOV_ED,     TOV_CENA_PDV,   TOV_KOLVO_PART, TOVAR_ID,       CURR_TOV_CENA,      CURR_TOV_CENA_PDV,      SKLAD_ID,       TOV_SUMA,   CURR_TOV_SUMA, DOC_DOPOLN, IS_PDV, COMPL_ID, IS_COMPL, COMPL_KOLVO_DEF, COMPL_IS_CONST, TOV_SUMA_ZNIG, CURR_TOV_SUMA_ZNIG)
                                                          VALUES (NULL, {0},    '{1}',      {2},        {3},        '{4}',      {5},            1,              {6},            {7},                {8},                    {9},            {10},       {11}, '', -1, 0, 0, 0, 0, 0, 0);";

        private const string insertTOVAR_MOVE = @"INSERT INTO TOVAR_MOVE (NUM,      DOC_TYPE_ID,    DOC_ID,     MDATE,      TOVAR_ID,   FROM_FIRMA_ID,      FROM_SKLAD_ID,      FROM_KOLVO,     FROM_CENA,      FROM_SUMA,      TO_FIRMA_ID,        TO_SKLAD_ID,        TO_KOLVO,       TO_CENA,        TO_SUMA,        CURR_TYPE,      CURR_FROM_CENA,         CURR_FROM_SUMA,         CURR_TO_CENA,   CURR_TO_SUMA,   FROM_FIRMA_RAH_ID, TO_FIRMA_RAH_ID, CENA_ZNIG_DIFF, SUMA_ZNIG_DIFF, CURR_CENA_ZNIG_DIFF, CURR_SUMA_ZNIG_DIFF,       CENA_PDV, SUMA_PDV, CURR_CENA_PDV, CURR_SUMA_PDV,       ARTICLE_ID, COMPL_ID, IS_USLUGA)
                                                                  VALUES (NULL,     1,              {0},        '{1}',      {2},        {3},                {4},                {5},            {6},            {7},            {8},                {9},                {10},           {11},           {12},           0,              {13},                   {14},                   {15},           {16},           0, 0, 0, 0, 0, 0,                                                                                                   {17}, {18}, {19}, {20},                                 0, 0, 0);";

        private const string updateBillPrice = @"UPDATE VNAKL SET CENA = {0}, CENA_PDV = {1}, PDV = {2}, CURR_CENA = {3}, CURR_CENA_PDV = {4}, CURR_PDV = {5} WHERE (NUM = {6});";

        private const string dateFormat = "yyyy-MM-dd hh:mm:ss";


        public void createBill(Client fromClient, Client toClient, PriceType priceType, Sklad sklad, List<InputTovar> tovars)
        {
            createBillHeader(getBiggestBillNumber() + 1, sklad.ID, fromClient.ID, toClient.ID);
            int billID = getLastBillID();
            decimal total = 0;
            foreach (InputTovar InputTovar in tovars)
            {
                total += addTovarToBill(billID, InputTovar.TovarKOD, InputTovar.Count, priceType, sklad.ID, fromClient.ID, toClient.ID);
            }

            setTotal(billID, total);
        }

        private void setTotal(int billID, decimal total)
        {
            string updateSQL = updateBillPriceSQL(billID, total);
            FbCommand command = new FbCommand(updateSQL, connection);
            command.ExecuteNonQuery();
        }

        private decimal addTovarToBill(int billID, string tovarID, double count, PriceType priceType, int skladID, int fromClientID, int toClientID)
        {
            UkrskladTovar InputTovar = getTovarByKOD(tovarID);
            decimal price = getPrice(InputTovar, priceType);
            decimal pricePDV = PDVUtility.getPriceWithPDV(price);
            decimal sum = (decimal)count * price;

            string billContentSQL = createBillContentSQL(billID, InputTovar.Name, InputTovar.ID, InputTovar.MeasurementUnits, count, price, pricePDV, price, pricePDV, skladID, sum, sum);
            FbCommand command = new FbCommand(billContentSQL, connection);
            command.ExecuteNonQuery();

            createTovarMove(billID, InputTovar.ID, fromClientID, skladID, count, price, toClientID);
            //update

            return sum;
        }

        private void createTovarMove(int billID, int tovarID, int fromFirmaID, int fromSkladID, double count, decimal price, int toFirmaID)
        {
            decimal sum = (decimal)count * price;
            decimal pricePDV = PDVUtility.getPDV(price);
            decimal sumPDV = PDVUtility.getPDV(sum);
            int toSkladId = 0; //because it is out 
            string insertTovarMove = createTovarMoveSQL(billID, DateTime.Today, tovarID, fromFirmaID, fromSkladID, count, price, sum, toFirmaID, toSkladId, count, price, sum, price, sum, price, sum, pricePDV, sumPDV, pricePDV, sumPDV);
            FbCommand command = new FbCommand(insertTovarMove, connection);
            command.ExecuteNonQuery();
        }

        private void createBillHeader(int billNumber, int skladID, int fromClientID, int toClientID)
        {
            int userID = 1;

            string clientName = getClientNameByID(toClientID);
            string billSQL = createBillSQL(fromClientID, billNumber, DateTime.Today, clientName, 0, 0, toClientID, 0, billNumber, skladID, 0, 0, 0, userID, DateTime.Now, DateTime.Now);
            FbCommand command = new FbCommand(billSQL, connection);
            command.ExecuteNonQuery();
        }

        private int getLastBillID()
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

        private int getBiggestBillNumber()
        {
            int biggestBillNumber = -1;
            string sql = "select * from VNAKL ORDER BY NU DESC";
            FbCommand command = new FbCommand(sql, connection);
            FbDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        biggestBillNumber = reader.GetInt32(2);
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

            return biggestBillNumber;
        }

        private string getClientNameByID( int clientID)
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

        private UkrskladTovar getTovarByKOD(string tovarKOD)
        {
            UkrskladTovar InputTovar = null;
            string sql = string.Format("select * from TOVAR_NAME where KOD='{0}'", tovarKOD);
            FbCommand command = new FbCommand(sql, connection);
            FbDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        InputTovar = new UkrskladTovar();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string column = reader.GetName(i);
                            if (column == "NUM")
                            {
                                InputTovar.ID = reader.GetInt32(i);
                            }
                            if (column == "NAME")
                            {
                                InputTovar.Name = reader.GetString(i);
                            }
                            if (column == "ED_IZM")
                            {
                                InputTovar.MeasurementUnits = reader.GetString(i);
                            }
                            if (column == "CENA")
                            {
                                InputTovar.Cina = reader.GetDecimal(i);
                            }
                            if (column == "CENA_O")
                            {
                                InputTovar.CinaOptova = reader.GetDecimal(i);
                            }
                            if (column == "CENA_R")
                            {
                                InputTovar.CinaRozdrib = reader.GetDecimal(i);
                            }

                            if (column == "CENA_1")
                            {
                                InputTovar.Cina1 = reader.GetDecimal(i);
                            }
                            if (column == "CENA_2")
                            {
                                InputTovar.Cina2 = reader.GetDecimal(i);
                            }
                            if (column == "CENA_3")
                            {
                                InputTovar.Cina3 = reader.GetDecimal(i);
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

            return InputTovar;
        }

        private string createBillSQL(int firmaID, int billNumber, DateTime billDate, string client, decimal price, decimal totalPrice, int clientID, decimal pdv, int nuID, int skladID, decimal currentPrice, decimal currentTotalPrice, decimal currentPdv, int userID, DateTime billCreated, DateTime billModified)
        {
            return string.Format(insertVNAKL, firmaID, billNumber, billDate.ToString(dateFormat), client, price, totalPrice, clientID, pdv, nuID, skladID, currentPrice, currentTotalPrice, currentPdv, userID, billCreated.ToString(dateFormat), billModified.ToString(dateFormat));
        }

        private string createBillContentSQL(int billID, string tovarName, int tovarID, string odVymiru, double count, decimal price, decimal totalPrice, decimal currentPrice, decimal currentTotalPrice, int skladID, decimal sumPrice, decimal currentSumPrice)
        {
            return string.Format(insertVNAKL_, billID, tovarName, count, price, odVymiru, totalPrice, tovarID, currentPrice, currentTotalPrice, skladID, sumPrice, currentSumPrice);
        }

        private string createTovarMoveSQL(int billID, DateTime date, int tovarID, int FromFirmaID, int FromSkladID, double fromCount, decimal fromPrice, decimal fromSum, int toFirmaID, int toSkladID, double toCount, decimal toPrice, decimal toSum, decimal currentFromPrice, decimal currentFromSum, decimal currentToPrice, decimal currentToSum, decimal pricePDV, decimal sumPDV, decimal currentPricePDV, decimal currentSumPDV)
        {
            return string.Format(insertTOVAR_MOVE, billID, date.ToString(dateFormat), tovarID, FromFirmaID, FromSkladID, fromCount, fromPrice, fromSum, toFirmaID, toSkladID, toCount, toPrice, toSum, currentFromPrice, currentFromSum, currentToPrice, currentToSum, pricePDV, sumPDV, currentPricePDV, currentSumPDV);
        }

        private string updateBillPriceSQL(int billID, decimal price)
        {
            return string.Format(updateBillPrice, price, PDVUtility.getPriceWithPDV(price), PDVUtility.getPDV(price), price, PDVUtility.getPriceWithPDV(price), PDVUtility.getPDV(price), billID);
        }

        private decimal getPrice(UkrskladTovar InputTovar, PriceType priceType)
        {
            decimal result = 0;
            switch (priceType)
            {
                case PriceType.Cina: result = InputTovar.Cina; break;
                case PriceType.Cina1: result = InputTovar.Cina1; break;
                case PriceType.Cina2: result = InputTovar.Cina2; break;
                case PriceType.Cina3: result = InputTovar.Cina3; break;
                case PriceType.CinaOptova: result = InputTovar.CinaOptova; break;
                case PriceType.CinaRozdrib: result = InputTovar.CinaRozdrib; break;
            }

            return result;
        }

        public void Dispose()
        {
            connection.Dispose();
        }

        public void Close() 
        {
            connection.Close();
        }


        public IList<Client> GetClients()
        {
            List<Client> clients = new List<Client>();
            string sql = "select * from CLIENT where VISIBLE=1";
            FbCommand command = new FbCommand(sql, connection);
            FbDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Client client = new Client();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetName(i) == "NUM")
                            {
                                client.ID = reader.GetInt32(i);
                            }
                            if (reader.GetName(i) == "FIO")
                            {
                                client.Name = reader.GetString(i);
                            }
                        }
                        clients.Add(client);
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

            return clients;
        }

        public IList<Sklad> GetSklads()
        {
            List<Sklad> sklads = new List<Sklad>();
            string sql = "select * from SKLAD_NAMES where VISIBLE=1";
            FbCommand command = new FbCommand(sql, connection);
            FbDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Sklad sklad = new Sklad();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetName(i) == "NUM")
                            {
                                sklad.ID = reader.GetInt32(i);
                            }
                            if (reader.GetName(i) == "NAME")
                            {
                                sklad.Name = reader.GetString(i);
                            }
                        }
                        sklads.Add(sklad);
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

            return sklads;
        }


        public string GetTovarName(string kod)
        {
            return getTovarByKOD(kod).Name;
        }
    }
}
