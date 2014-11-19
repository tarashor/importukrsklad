﻿using FirebirdSql.Data.FirebirdClient;
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
    public class UkrskladDB5 : UkrskladDB
    {
        public UkrskladDB5(string host, string databaseLocation, bool isLocal, ILogger logger) : base(host, databaseLocation, isLocal, logger) { }

        private const string insertVNAKL = @"INSERT INTO VNAKL (NUM,    FIRMA_ID,   NU,     DATE_DOK,    CLIENT,             CENA,       CENA_PDV,       CENA_ZNIG,      DOV_SER,        DOV_NUM,        DOV_DATA,       DOV_FIO,        CLIENT_ID,      PDV,        PROD_UMOV,      ZNIG_TYPE,      POD_REKL_PER,       POD_REKL_CH,   FIO_BUH,    NU_ID,      SKLAD_ID,   CURR_TYPE,      CURR_CENA,      CURR_CENA_PDV,      CURR_CENA_ZNIG,     CURR_PDV,       CURR_POD_REKL_CH,   CLIENT_RAH_ID,  AFIRM_RAH_ID,       DOPOLN,         CENA_TOV_TRANS, CURR_CENA_TOV_TRANS,    IS_MOVE,    DOC_MARK_TYPE,      DOC_USER_ID,        DOC_CREATE_TIME,    DOC_MODIFY_TIME,    DOC_DESCR,      ARTICLE_ID, TTN_NU, TTN_DATE_DOK, TTN_DOR_LIST, TTN_AVTO, TTN_AVTO_PIDPR, TTN_VODITEL, TTN_ADR_FROM, TTN_ADR_TO, IM_NUM, ZNIG_PROC, TTN_AVTO_PRIC, TTN_VID_PEREV, TTN_KOLVO_MIS, TTN_MAS_BRUT, PDV_TYPE)
                                                        VALUES (NULL,   {0},        '{1}',  '{2}',       '{3}',              {4},        {5},            0,              '',             '',             NULL,           '',             {6},            {7},        '',             0,              NULL,               0,             NULL,       {8},        {9},        0,              {10},           {11},               0,                  {12},           0,                  0,              0,                  '',             0, 0,                                   1,          0,                  {13},               '{14}',             '{15}',             '',             0, '', '1899-12-30 00:00:00', '', '', '', '', '', '', -1, 0, '', '', '', '', 0);";

        private const string insertVNAKL_ = @"INSERT INTO VNAKL_ (NUM,  PID,    TOV_NAME,   TOV_KOLVO,  TOV_CENA,   TOV_ED,     TOV_CENA_PDV,   TOV_KOLVO_PART, TOVAR_ID,       CURR_TOV_CENA,      CURR_TOV_CENA_PDV,      SKLAD_ID,       TOV_SUMA,   CURR_TOV_SUMA, DOC_DOPOLN, IS_PDV, COMPL_ID, IS_COMPL, COMPL_KOLVO_DEF, COMPL_IS_CONST, TOV_SUMA_ZNIG, CURR_TOV_SUMA_ZNIG)
                                                          VALUES (NULL, {0},    '{1}',      {2},        {3},        '{4}',      {5},            1,              {6},            {7},                {8},                    {9},            {10},       {11}, '', -1, 0, 0, 0, 0, 0, 0);";

        private const string insertTOVAR_MOVE = @"INSERT INTO TOVAR_MOVE (NUM,      DOC_TYPE_ID,    DOC_ID,     MDATE,      TOVAR_ID,   FROM_FIRMA_ID,      FROM_SKLAD_ID,      FROM_KOLVO,     FROM_CENA,      FROM_SUMA,      TO_FIRMA_ID,        TO_SKLAD_ID,        TO_KOLVO,       TO_CENA,        TO_SUMA,        CURR_TYPE,      CURR_FROM_CENA,         CURR_FROM_SUMA,         CURR_TO_CENA,   CURR_TO_SUMA,   FROM_FIRMA_RAH_ID, TO_FIRMA_RAH_ID, CENA_ZNIG_DIFF, SUMA_ZNIG_DIFF, CURR_CENA_ZNIG_DIFF, CURR_SUMA_ZNIG_DIFF,       CENA_PDV, SUMA_PDV, CURR_CENA_PDV, CURR_SUMA_PDV,       ARTICLE_ID, COMPL_ID, IS_USLUGA)
                                                                  VALUES (NULL,     1,              {0},        '{1}',      {2},        {3},                {4},                {5},            {6},            {7},            {8},                {9},                {10},           {11},           {12},           0,              {13},                   {14},                   {15},           {16},           0, 0, 0, 0, 0, 0,                                                                                                   {17}, {18}, {19}, {20},                                 0, 0, 0);";

        private const string insertTOVAR_ZAL = @"INSERT INTO TOVAR_ZAL (NUM,    FIRMA_ID,   TOVAR_ID,       SKLAD_ID,       KOLVO,      SUMA,       CENA_IN,    CENA_R,     CENA_O,     CENA_1,     CENA_2,       LAST_POST_ID)
                                                                VALUES (NULL,   {0},        {1},            {2},            {3},        0,          {4},        {5},        {6},        {7},        {8},          -1);";

        private const string updateTOVAR_ZAL = @"UPDATE TOVAR_ZAL SET KOLVO = {0} WHERE (NUM = {1});";

        private const string updateBillPrice = @"UPDATE VNAKL SET CENA = {0}, CENA_PDV = {1}, PDV = {2}, CURR_CENA = {3}, CURR_CENA_PDV = {4}, CURR_PDV = {5} WHERE (NUM = {6});";

        private const string updateClientSuma = @"UPDATE CLIENT SET CLIENT_SUMA = {0} WHERE (NUM = {1});";

        private const string dateTimeFormat = "yyyy-MM-dd hh:mm:ss";

        private const string dateFormat = "yyyy-MM-dd 00:00:00";

        

        protected override string createBillSQL(int firmaID, int billNumber, DateTime billDate, string client, decimal price, decimal totalPrice, int clientID, decimal pdv, int nuID, int skladID, decimal currentPrice, decimal currentTotalPrice, decimal currentPdv, int userID, DateTime billCreated, DateTime billModified)
        {
            return string.Format(insertVNAKL, firmaID, billNumber, billDate.ToString(dateFormat), client, price, totalPrice, clientID, pdv, nuID, skladID, currentPrice, currentTotalPrice, currentPdv, userID, billCreated.ToString(dateTimeFormat), billModified.ToString(dateTimeFormat));
        }

        protected override string createBillContentSQL(int billID, string tovarName, int tovarID, string odVymiru, double count, decimal price, decimal totalPrice, decimal currentPrice, decimal currentTotalPrice, int skladID, decimal sumPrice, decimal currentSumPrice)
        {
            return string.Format(insertVNAKL_, billID, tovarName, count, price, odVymiru, totalPrice, tovarID, currentPrice, currentTotalPrice, skladID, sumPrice, currentSumPrice);
        }

        protected override string createTovarMoveSQL(int billID, DateTime date, int tovarID, int FromFirmaID, int FromSkladID, double fromCount, decimal fromPrice, decimal fromSum, int toFirmaID, int toSkladID, double toCount, decimal toPrice, decimal toSum, decimal currentFromPrice, decimal currentFromSum, decimal currentToPrice, decimal currentToSum, decimal pricePDV, decimal sumPDV, decimal currentPricePDV, decimal currentSumPDV)
        {
            return string.Format(insertTOVAR_MOVE, billID, date.ToString(dateTimeFormat), tovarID, FromFirmaID, FromSkladID, fromCount, fromPrice, fromSum, toFirmaID, toSkladID, toCount, toPrice, toSum, currentFromPrice, currentFromSum, currentToPrice, currentToSum, pricePDV, sumPDV, currentPricePDV, currentSumPDV);
        }

        protected override string updateBillPriceSQL(int billID, decimal price)
        {
            return string.Format(updateBillPrice, price, PDVUtility.getPriceWithPDV(price), PDVUtility.getPDV(price), price, PDVUtility.getPriceWithPDV(price), PDVUtility.getPDV(price), billID);
        }

        protected override string getUpdateClientSumSQL(int clientID, decimal sum)
        {
            return string.Format(updateClientSuma, sum, clientID);
        }

        protected override string getUpdateTovarLeftSQL(int leftID, double leftCount)
        {
            return string.Format(updateTOVAR_ZAL, leftCount, leftID);
        }

        protected override string getInsertTovarLeftSQL(int tovarID, int skladID, int fromClientID, double leftCount, decimal cina, decimal cinar, decimal cinao, decimal cina1, decimal cina2)
        {
            return string.Format(insertTOVAR_ZAL, fromClientID, tovarID, skladID, leftCount, cina, cinar, cinao, cina1, cina2);
        }

        protected override string getSelectTovarLeftSQL(int tovarID, int skladID, int clientID)
        {
            return string.Format("select * from TOVAR_ZAL where FIRMA_ID={0} and TOVAR_ID={1} and SKLAD_ID={2}", clientID, tovarID, skladID);
        }

        protected override string getSelectVNAKLOrderedByNumDESC()
        {
            return "select * from VNAKL ORDER BY NUM DESC";
        }

        protected override string getSelectVNAKLOrderedByNUDESC()
        {
            return "select * from VNAKL ORDER BY NU DESC";
        }

        protected override string getSelectClientNameByID(int clientID)
        {
            return "select * from CLIENT where NUM=" + clientID.ToString();
        }

        protected override string getSelectTovarByKODSQL(string tovarKOD)
        {
            return string.Format("select * from TOVAR_NAME where KOD='{0}'", tovarKOD);
        }

        protected override string getSelectVisibleClientsSQL()
        {
            return "select * from CLIENT where VISIBLE=1";
        }

        protected override string getSelectVisibleSkladsSQL()
        {
            return "select * from SKLAD_NAMES where VISIBLE=1";
        }

        protected override void setAdditionalValues(int userID, Client fromClient, Client toClient, PriceType priceType, Sklad sklad, List<InputTovar> tovars, decimal total)
        {
            setTotalForClient(toClient.ID, total);
        }

        private void setTotalForClient(int clientID, decimal total)
        {
            decimal sum = getClientSum(clientID);
            sum -= total;
            string updateSQL = getUpdateClientSumSQL(clientID, sum);
            executeNonQuery(updateSQL);
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
    }
}
