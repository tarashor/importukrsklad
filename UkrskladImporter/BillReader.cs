using ExportScanner.Domain.Model;
using ExportScanner.Domain.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ukrsklad.Domain;
using Ukrsklad.Domain.Model;

namespace UkrskladImporter
{
    public class BillReader
    {
        private IList<Client> clients;
        private IList<Client> activeFirms;
        private int defaultFromClientID;

        public BillReader(IList<Client> clients, IList<Client> activeFirms, int defaultFromClientID) 
        { 
            this.clients = clients;
            this.activeFirms = activeFirms;
            this.defaultFromClientID = defaultFromClientID;
        }

        public Bill ReadFromFile(string filename)
        {
            BillScanner billFromScanner =  BillScannerSerializator.Load(filename);
            return convertBill(billFromScanner);
        }

        private Bill convertBill(BillScanner billfromScanner)
        {
            Bill bill = new Bill();
            int clientID = mapScannerClientIdToUkrskladId(billfromScanner.ClientID);
            foreach(int scannerTovarId in billfromScanner.Tovars.Keys)
            {
                bill.Tovars.Add(createTovar(scannerTovarId, billfromScanner.Tovars[scannerTovarId]));
            }

            bill.FromClient = getClient(defaultFromClientID);
            bill.ToClient = getClient(clientID);
            return bill;
        }

        

        private Client getClient(int clientID)
        {
            foreach (Client client in clients) { 
                if (client.ID == clientID)
                    return client;
            }
            return null;
        }

        private Client getActiveFirm(int activeFirmID)
        {
            foreach (Client client in activeFirms)
            {
                if (client.ID == activeFirmID)
                    return client;
            }
            return null;
        }

        private Tovar createTovar(int scannerId, double count) { 
            Tovar tovar = new Tovar();
            tovar.Count = count;
            tovar.KOD = mapScannerIdToKod(scannerId);
            return tovar;
        }

        private string mapScannerIdToKod(int scannerId)
        {
            return scannerId.ToString();
        }

        private int mapScannerClientIdToUkrskladId(int scannerClientID)
        {
            return -20;
        }
    }
}
