using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ukrsklad.Domain;
using Ukrsklad.Domain.Model;

namespace UkrskladImporter
{
    public partial class Form1 : Form
    {
        private const string dbLocation = @"c:\ProgramData\UkrSklad\db\Sklad.tcb";
        private Bill bill;
        private UkrskladDB db;
        private IList<Client> clients;
        private IList<Client> activeFirms;
        private IList<Sklad> sklads;
        private IList<Price> prices;

        public Form1()
        {
            InitializeComponent();
            db = new UkrskladDB(dbLocation);
            clients = db.GetClients();
            activeFirms = db.GetClients();
            sklads = db.GetSklads();
            prices = getPrices();
        }

        private IList<Price> getPrices()
        {
            List<Price> prices = new List<Price>();
            List<PriceType> priceTypes = Enum.GetValues(typeof(PriceType)).Cast<PriceType>().ToList();
            foreach (PriceType type in priceTypes)
            { 
                Price price = new Price();
                price.Name = type.ToString();
                price.Value = type;
                prices.Add(price);
            }
            return prices;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK) {
                loadBill(BillReader.ReadFromFile(openFileDialog.FileName, clients, activeFirms));
            }
        }

        private void loadBill(Bill bill) {
            this.bill = bill;
            foreach (Tovar tovar in bill.Tovars) {
                tovar.Name = db.GetTovarName(tovar.KOD);
            }
            tovars.DataSource = bill.Tovars;
            activeFirmComboBox.SelectedItem = bill.FromClient;
            clientsComboBox.SelectedItem = bill.ToClient;
            skladsComboBox.SelectedItem = bill.Sklad;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            activeFirmComboBox.DataSource = activeFirms;
            activeFirmComboBox.DisplayMember = "Name";

            clientsComboBox.DataSource = clients;
            clientsComboBox.DisplayMember = "Name";

            skladsComboBox.DataSource = sklads;
            skladsComboBox.DisplayMember = "Name";

            pricesComboBox.DataSource = prices;
            pricesComboBox.DisplayMember = "Name";
            pricesComboBox.ValueMember = "Value";

            tovars.AutoGenerateColumns = false;
            tovars.AutoSize = true;

            DataGridViewColumn columnKod = new DataGridViewTextBoxColumn();
            columnKod.DataPropertyName = "KOD";
            columnKod.Name = "Код";
            tovars.Columns.Add(columnKod);

            DataGridViewColumn columnName = new DataGridViewTextBoxColumn();
            columnName.DataPropertyName = "Name";
            columnName.Name = "Назва";
            tovars.Columns.Add(columnName);

            DataGridViewColumn columnCount = new DataGridViewTextBoxColumn();
            columnCount.DataPropertyName = "Count";
            columnCount.Name = "Маса";
            tovars.Columns.Add(columnCount);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bill != null)
            {
                db.createBill(bill.FromClient, bill.ToClient, (PriceType)pricesComboBox.SelectedValue, bill.Sklad, convertTovars(bill.Tovars));
            }
        }

        private List<InputTovar> convertTovars(IList<Tovar> list)
        {
            List<InputTovar> inputTovars = new List<InputTovar>();
            foreach (Tovar tovar in list) {
                inputTovars.Add(new InputTovar() { Count = tovar.Count, TovarKOD = tovar.KOD });
            }
            return inputTovars;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.Close();
        }

        private void clientsComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (bill != null)
                bill.ToClient = (Client)clientsComboBox.SelectedValue;
        }

        private void skladsComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (bill != null)
                bill.Sklad = (Sklad)skladsComboBox.SelectedValue;
        }

        private void activeFirmComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (bill != null)
                bill.FromClient = (Client)activeFirmComboBox.SelectedValue;
        }



        

        
    }
}
