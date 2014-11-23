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
        private IList<Sklad> sklads;
        private IDictionary<int, int> clientMapScannerToUkrsklad;
        private IDictionary<int, string> goodsMapScannerToUkrsklad;
        private int defaultFromClientID;
        private int defaultSkladID;

        public string Log { get; set; }

        public BillReader(IList<Client> clients, IList<Client> activeFirms, IList<Sklad> sklads, int defaultFromClientID, int defaultSkladID, IDictionary<int, string> goodsMapScannerToUkrsklad, IDictionary<int, int> clientMapScannerToUkrsklad) 
        { 
            this.clients = clients;
            this.activeFirms = activeFirms;
            this.sklads = sklads;
            this.defaultFromClientID = defaultFromClientID;
            this.defaultSkladID = defaultSkladID;
            this.clientMapScannerToUkrsklad = clientMapScannerToUkrsklad;
            this.goodsMapScannerToUkrsklad = goodsMapScannerToUkrsklad;
        }

        public Bill ReadFromFile(string filename)
        {
            BillScanner billFromScanner =  BillScannerSerializator.Load(filename);
            return convertBill(billFromScanner);
        }

        private Bill convertBill(BillScanner billfromScanner)
        {
            Bill bill = new Bill();
            Log = string.Empty;
            try
            {
                int clientID = mapScannerClientIdToUkrskladId(billfromScanner.ClientID);
                bill.ToClient = getClient(clientID);
            }
            catch (ArgumentException ae) {
                Log += ae.Message + "\r\n";
            }

            foreach(int scannerTovarId in billfromScanner.Tovars.Keys)
            {
                try
                {
                    Tovar tovar = createTovar(scannerTovarId, billfromScanner.Tovars[scannerTovarId]);
                    bill.Tovars.Add(tovar);
                }
                catch (ArgumentException ae)
                {
                    Log += ae.Message + "\r\n";
                }
            }

            bill.CreationDate = billfromScanner.OutDate;
            bill.FromClient = getActiveFirm(defaultFromClientID);
            bill.Sklad = getSklad(defaultSkladID);
            
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

        private Sklad getSklad(int skladId)
        {
            foreach (Sklad sklad in sklads)
            {
                if (sklad.ID == skladId)
                    return sklad;
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
            string ukrskladID;
            if (goodsMapScannerToUkrsklad.TryGetValue(scannerId, out ukrskladID)) 
            {
                return ukrskladID;
            }
            throw new ArgumentException("Немає відповідності у файлі для товару з кодом:" + scannerId.ToString());
            
        }

        private int mapScannerClientIdToUkrskladId(int scannerClientID)
        {
            int ukrskladID;
            if (clientMapScannerToUkrsklad.TryGetValue(scannerClientID, out ukrskladID))
            {
                return ukrskladID;
            }
            throw new ArgumentException("Немає відповідності у файлі для клієнта з кодом:" + scannerClientID.ToString());
        }
    }
}
