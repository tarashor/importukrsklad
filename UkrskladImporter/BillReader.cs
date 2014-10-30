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
        public static Bill ReadFromFile(string filename, IList<Client> clients, IList<Client> activeFirm)
        {
            Bill bill = new Bill();

            //int firmID = 1;
            int clientID = -20;

            bill.Tovars.Add(createTovar("111", 11.2));
            bill.Tovars.Add(createTovar("111", 5));
            bill.Tovars.Add(createTovar("111", 1));

            //bill.FromClient = getClient(activeFirm, firmID);
            bill.ToClient = getClient(clients, clientID);
            
            return bill;
        }

        private static Client getClient(IList<Client> clients, int clientID)
        {
            foreach (Client client in clients) { 
                if (client.ID == clientID)
                    return client;
            }
            return null;
        }

        private static Tovar createTovar(string kod, double count) { 
            Tovar tovar = new Tovar();
            tovar.Count = count; 
            tovar.KOD = kod;
            return tovar;
        }
    }
}
