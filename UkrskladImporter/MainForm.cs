﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ukrsklad.Domain;
using Ukrsklad.Domain.Model;

namespace UkrskladImporter
{
    public partial class MainForm : Form
    {
        private Bill bill;
        private UkrskladDB db;
        private IList<Client> clients;
        private IList<Client> activeFirms;
        private IList<Sklad> sklads;
        private IList<Price> prices;
        private BillReader billReader;
        private int userID;

        public MainForm()
        {
            InitializeComponent();
            try
            {
                string dbLocation = ConfigurationManager.AppSettings["DBLocation"];
                bool isLocal = ConfigurationManager.AppSettings["DBIsLocal"] == "1" ? true : false;
                db = new UkrskladDB(dbLocation, isLocal);
                clients = db.GetClients();
                activeFirms = db.GetClients();
                sklads = db.GetSklads();
                prices = getPrices();
            }
            catch (Exception e) {
                MessageBox.Show("Відсутнє підключення до бази даних укрскладу. Перевірте шлях до бази даних в конфігураційному файлі.");
            }

            if (!int.TryParse(ConfigurationManager.AppSettings["applicationUserID"], out userID)) 
            {
                MessageBox.Show("Задайте вірно користувача під яким будуть створюватись документи.");
            }

            int defaultActiveFirm = getDefaultActiveFirm();

            string scannerToUkrskladClientsFile = ConfigurationManager.AppSettings["scannerToUkrskladClients"];
            string scannerToUkrskladGoodsFile = ConfigurationManager.AppSettings["scannerToUkrskladGoods"];

            IDictionary<int, int> clientMap = getClientMapScannerToUkrsklad(scannerToUkrskladClientsFile);
            IDictionary<int, string> goodsMap = getGoodsMapScannerToUkrsklad(scannerToUkrskladGoodsFile);

            billReader = new BillReader(clients, activeFirms, defaultActiveFirm, goodsMap, clientMap);
        }

        private static int getDefaultActiveFirm()
        {
            int defaultActiveFirm = 1;
            try
            {
                defaultActiveFirm = int.Parse(ConfigurationManager.AppSettings["defaultActiveFirmID"]);
            }
            catch (Exception e)
            {
                MessageBox.Show("Неправильно задана активна фірма у конфігураційному файлі.");
            }

            return defaultActiveFirm;
        }

        private IDictionary<int, int> getClientMapScannerToUkrsklad(string filename)
        {
            IDictionary<int, int> map = new Dictionary<int, int>();

            string[] lines = System.IO.File.ReadAllLines(filename);

            foreach (string line in lines)
            {
                string lineTrimed = line.Trim();
                if (!string.IsNullOrEmpty(lineTrimed))
                {
                    string[] parts = lineTrimed.Split('=');
                    
                    if (parts.Length == 2){
                        int key;
                        int value;
                        if (int.TryParse(parts[0].Trim(), out key) && (int.TryParse(parts[1].Trim(), out value)))
                        {
                            map.Add(key, value);
                        }
                    }
                }
            }

            return map;

        }

        private IDictionary<int, string> getGoodsMapScannerToUkrsklad(string filename)
        {
            IDictionary<int, string> map = new Dictionary<int, string>();

            string[] lines = System.IO.File.ReadAllLines(filename);

            foreach (string line in lines)
            {
                string lineTrimed = line.Trim();
                if (!string.IsNullOrEmpty(lineTrimed))
                {
                    string[] parts = lineTrimed.Split('=');

                    if (parts.Length == 2)
                    {
                        int key;
                        string value = parts[1].Trim();
                        if (int.TryParse(parts[0].Trim(), out key))
                        {
                            map.Add(key, value);
                        }
                    }
                }
            }

            return map;
        }


        private IList<Price> getPrices()
        {
            List<Price> prices = new List<Price>();
            List<PriceType> priceTypes = Enum.GetValues(typeof(PriceType)).Cast<PriceType>().ToList();
            foreach (PriceType type in priceTypes)
            { 
                Price price = new Price();
                price.Name = convertPriceTypeToString(type);
                price.Value = type;
                prices.Add(price);
            }
            return prices;
        }

        private string convertPriceTypeToString(PriceType type)
        {
            return ConfigurationManager.AppSettings[type.ToString()];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK) {
                try
                {
                    Bill b = billReader.ReadFromFile(openFileDialog.FileName);
                    loadBill(b);
                }
                catch (ArgumentException ae) {
                    MessageBox.Show(ae.Message);
                }
            }
        }

        private void loadBill(Bill bill) {
            this.bill = bill;
            /*foreach (Tovar tovar in bill.Tovars) {
                tovar.Name = db.GetTovarName(tovar.KOD);
            }*/
            tovars.DataSource = bill.Tovars;
            activeFirmComboBox.SelectedItem = bill.FromClient;
            clientsComboBox.SelectedItem = bill.ToClient;
            skladsComboBox.SelectedItem = bill.Sklad;

            enableClientCheckBox.Checked = false;
            enableFirmCheckBox.Checked = false;
            
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

            enableClientCheckBox.Checked = false;
            enableFirmCheckBox.Checked = false;
            activeFirmComboBox.Enabled = enableFirmCheckBox.Checked;
            clientsComboBox.Enabled = enableClientCheckBox.Checked;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bill != null)
            {
                db.createBill(userID, bill.FromClient, bill.ToClient, (PriceType)pricesComboBox.SelectedValue, bill.Sklad, convertTovars(bill.Tovars));
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
            if (db != null)
            {
                db.Close();
            }
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

        private void enableFirmCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            activeFirmComboBox.Enabled = enableFirmCheckBox.Checked;
        }

        private void enableClientCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            clientsComboBox.Enabled = enableClientCheckBox.Checked;
        }
    }
}