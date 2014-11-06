using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ukrsklad.Domain.Model;
using Ukrsklad.Domain.Utility;

namespace Ukrsklad.Domain
{
    public abstract class UkrskladDB : IUkrskladDB, IDisposable
    {
        protected FbConnection connection;

        protected UkrskladDB(string host, string databaseLocation, bool isLocal) {
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
            if (isLocal)
            {
                cs.ServerType = FbServerType.Embedded;
            }
            else
            {
                cs.DataSource = host;
                cs.Port = 3050;
            }

            connection = new FbConnection(cs.ToString());
            connection.Open();
        }

        public IList<Client> GetClients()
        {
            List<Client> clients = new List<Client>();
            string sql = getSelectVisibleClientsSQL();
            executeSelect(sql, (reader) =>
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
            });

            return clients;
        }

        

        public IList<Sklad> GetSklads()
        {
            List<Sklad> sklads = new List<Sklad>();
            string sql = getSelectVisibleSkladsSQL();
            executeSelect(sql, (reader) =>
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
            });
             

            return sklads;
        }

        

        public void createBill(int userID, Client fromClient, Client toClient, PriceType priceType, Sklad sklad, List<InputTovar> tovars)
        {
            createBillHeader(getBiggestBillNumber() + 1, sklad.ID, fromClient.ID, toClient.ID, userID);
            int billID = getLastBillID();
            decimal total = 0;
            foreach (InputTovar InputTovar in tovars)
            {
                total += addTovarToBill(billID, InputTovar.TovarKOD, InputTovar.Count, priceType, sklad.ID, fromClient.ID, toClient.ID);
            }

            setTotalInBill(billID, total);
            setTotalForClient(toClient.ID, total);
        }

        private void setTotalForClient(int clientID, decimal total)
        {
            decimal sum = getClientSum(clientID);
            sum -= total;
            string updateSQL = getUpdateClientSumSQL(clientID, sum);
            executeNonQuery(updateSQL);
        }

        private void setTotalInBill(int billID, decimal total)
        {
            string updateSQL = updateBillPriceSQL(billID, total);
            executeNonQuery(updateSQL);
        }

        private decimal addTovarToBill(int billID, string tovarID, double count, PriceType priceType, int skladID, int fromClientID, int toClientID)
        {
            UkrskladTovar InputTovar = getTovarByKOD(tovarID);
            decimal price = getPrice(InputTovar, priceType);
            decimal pricePDV = PDVUtility.getPriceWithPDV(price);
            decimal sum = (decimal)count * price;

            string billContentSQL = createBillContentSQL(billID, InputTovar.Name, InputTovar.ID, InputTovar.MeasurementUnits, count, price, pricePDV, price, pricePDV, skladID, sum, sum);
            executeNonQuery(billContentSQL);

            createTovarMove(billID, InputTovar.ID, fromClientID, skladID, count, price, toClientID);
            updateTovarLeft(InputTovar.ID, skladID, fromClientID, count, InputTovar.Cina, InputTovar.CinaRozdrib, InputTovar.CinaOptova, InputTovar.Cina1, InputTovar.Cina2);

            return sum;
        }

        private void updateTovarLeft(int tovarID, int skladID, int fromClientID, double count, decimal cina, decimal cinar, decimal cinao, decimal cina1, decimal cina2)
        {
            double leftCount;
            int? leftID = getLeftCountOfTovar(tovarID, skladID, fromClientID, out leftCount);
            if (leftID.HasValue)
            {
                leftCount -= count;
                updateLeftCountTovar(leftID.Value, leftCount);
            }
            else 
            {
                insertLeftCountTovar(tovarID, skladID, fromClientID, leftCount, cina, cinar, cinao, cina1, cina2);
            }
        }

        private void updateLeftCountTovar(int leftID, double leftCount)
        {
            string updateLeftSQL = getUpdateTovarLeftSQL(leftID, leftCount);
            executeNonQuery(updateLeftSQL);
        }

        private void insertLeftCountTovar(int tovarID, int skladID, int fromClientID, double leftCount, decimal cina, decimal cinar, decimal cinao, decimal cina1, decimal cina2)
        {
            string insertLeftSQL = getInsertTovarLeftSQL(tovarID, skladID, fromClientID, leftCount, cina, cinar, cinao, cina1, cina2);
            executeNonQuery(insertLeftSQL);
        }

        private void createTovarMove(int billID, int tovarID, int fromFirmaID, int fromSkladID, double count, decimal price, int toFirmaID)
        {
            decimal sum = (decimal)count * price;
            decimal pricePDV = PDVUtility.getPDV(price);
            decimal sumPDV = PDVUtility.getPDV(sum);
            int toSkladId = 0; //because it is out 
            string insertTovarMove = createTovarMoveSQL(billID, DateTime.Today, tovarID, fromFirmaID, fromSkladID, count, price, sum, toFirmaID, toSkladId, count, price, sum, price, sum, price, sum, pricePDV, sumPDV, pricePDV, sumPDV);
            executeNonQuery(insertTovarMove);
        }

        private void createBillHeader(int billNumber, int skladID, int fromClientID, int toClientID, int userID)
        {
            string clientName = getClientNameByID(toClientID);
            string billSQL = createBillSQL(fromClientID, billNumber, DateTime.Today, clientName, 0, 0, toClientID, 0, billNumber, skladID, 0, 0, 0, userID, DateTime.Now, DateTime.Now);
            executeNonQuery(billSQL);
        }

        private int? getLeftCountOfTovar(int tovarID, int skladID, int clientID, out double leftCount)
        {
            double count = 0;
            int? leftID = null;
            string sql = getSelectTovarLeftSQL(tovarID, skladID, clientID);
            executeSelectOneRow(sql, (reader) =>
            {
                leftID = reader.GetInt32(0); 
                count = reader.GetDouble(4);
            });
            leftCount = count;
            return leftID;
        }
        

        private int getLastBillID()
        {
            int lastBillID = -1;
            string sql = getSelectVNAKLOrderedByNumDESC();
            executeSelectOneRow(sql, (reader) =>
            {
                lastBillID = reader.GetInt32(0);
            });
            return lastBillID;
        }

        private int getBiggestBillNumber()
        {
            int biggestBillNumber = -1;
            string sql = getSelectVNAKLOrderedByNUDESC();
            executeSelectOneRow(sql, (reader) =>
            {
                biggestBillNumber = reader.GetInt32(2);
            });
            return biggestBillNumber;
        }

        private string getClientNameByID(int clientID)
        {
            string clientName = string.Empty;
            string sql = getSelectClientNameByID(clientID);
            executeSelectOneRow(sql, (reader) =>
             {
                 for (int i = 0; i < reader.FieldCount; i++)
                 {
                     if (reader.GetName(i) == "FIO")
                     {
                         clientName = reader.GetString(i);
                         break;
                     }
                 }
             });
            return clientName;
        }



        private decimal getClientSum(int clientID)
        {
            decimal clientSum = 0;
            string sql = getSelectClientNameByID(clientID);
            executeSelectOneRow(sql, (reader) =>
               {
                   for (int i = 0; i < reader.FieldCount; i++)
                   {
                       if (reader.GetName(i) == "CLIENT_SUMA")
                       {
                           clientSum = reader.GetDecimal(i);
                           break;
                       }
                   }
               });
            return clientSum;
        }

        private UkrskladTovar getTovarByKOD(string tovarKOD)
        {
            UkrskladTovar InputTovar = null;
            string sql = getSelectTovarByKODSQL(tovarKOD);
            executeSelectOneRow(sql, (reader) =>
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
              });

            return InputTovar;
        }



        protected abstract string createBillSQL(int firmaID, int billNumber, DateTime billDate, string client, decimal price, decimal totalPrice, int clientID, decimal pdv, int nuID, int skladID, decimal currentPrice, decimal currentTotalPrice, decimal currentPdv, int userID, DateTime billCreated, DateTime billModified);

        protected abstract string createBillContentSQL(int billID, string tovarName, int tovarID, string odVymiru, double count, decimal price, decimal totalPrice, decimal currentPrice, decimal currentTotalPrice, int skladID, decimal sumPrice, decimal currentSumPrice);

        protected abstract string createTovarMoveSQL(int billID, DateTime date, int tovarID, int FromFirmaID, int FromSkladID, double fromCount, decimal fromPrice, decimal fromSum, int toFirmaID, int toSkladID, double toCount, decimal toPrice, decimal toSum, decimal currentFromPrice, decimal currentFromSum, decimal currentToPrice, decimal currentToSum, decimal pricePDV, decimal sumPDV, decimal currentPricePDV, decimal currentSumPDV);

        protected abstract string updateBillPriceSQL(int billID, decimal price);

        protected abstract string getUpdateClientSumSQL(int clientID, decimal sum);

        protected abstract string getUpdateTovarLeftSQL(int leftID, double leftCount);

        protected abstract string getInsertTovarLeftSQL(int tovarID, int skladID, int fromClientID, double leftCount, decimal cina, decimal cinar, decimal cinao, decimal cina1, decimal cina2);

        protected abstract string getSelectTovarLeftSQL(int tovarID, int skladID, int clientID);
        
        protected abstract string getSelectVNAKLOrderedByNumDESC();

        protected abstract string getSelectVNAKLOrderedByNUDESC();

        protected abstract string getSelectClientNameByID(int clientID);
        
        protected abstract string getSelectTovarByKODSQL(string tovarKOD);
        
        protected abstract string getSelectVisibleClientsSQL();

        protected abstract string getSelectVisibleSkladsSQL();
        
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

        public string GetTovarName(string kod)
        {
            UkrskladTovar tovar = getTovarByKOD(kod);
            if (tovar != null){
                return tovar.Name;
            }
            throw new ArgumentException(string.Format("У базі даних немає товару з кодом {0}", kod));
        }

        private void executeSelectOneRow(string selectSQL, Action<DbDataReader> action)
        {
            FbCommand command = new FbCommand(selectSQL, connection);
            FbDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        action(reader);
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
        }

        private void executeSelect(string selectSQL, Action<DbDataReader> action)
        {
            FbCommand command = new FbCommand(selectSQL, connection);
            FbDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        action(reader);
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
        }

        private void executeNonQuery(string SQL)
        {
            FbCommand command = new FbCommand(SQL, connection);
            command.ExecuteNonQuery();
        }
    }
}
