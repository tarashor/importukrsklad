using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ukrsklad.Domain.Model;
using Ukrsklad.Domain.Utility;

namespace Ukrsklad.Domain
{
    public interface IUkrskladDB : IDisposable
    {
        void createBill(int userID, Client fromClient, Client toClient, PriceType priceType, Sklad sklad, List<InputTovar> tovars, DateTime creationDate, ILogger logger);
        string GetTovarName(string kod, Client client, Sklad sklad);
        IList<Client> GetClients();
        IList<Sklad> GetSklads();
        void Close();
    }
}
