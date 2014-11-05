using ExportScanner.Domain;
using ExportScanner.Domain.Model;
using ExportScanner.Domain.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;

namespace ScannerExporter
{
    public partial class MainForm : Form
    {
        private readonly IScannerDB db;
        private const string fileNameTemplate = @"{0}.vnakl";

        public MainForm()
        {
            InitializeComponent();
            string scannerDBLocation = ConfigurationManager.AppSettings["DBLocation"];
            db = new ScannerDB(scannerDBLocation);
        }

        private void load() {
            IEnumerable<int> clientsInBills = db.GetDistinctClientsInBill();
            List<BillScanner> bills = new List<BillScanner>();
            foreach (int clientId in clientsInBills) 
            {
                BillScanner bill = db.GetBillForClient(clientId);
                bills.Add(bill);
            }

            /*List<BillScanner> bills = new List<BillScanner>();
            BillScanner bill = new BillScanner();
            bill.ClientID = 4;
            bill.ClientName = "Консенсія";
            bill.OutDate = DateTime.Now;
            bill.Tovars.Add(2, 10);
            bill.Tovars.Add(3, 5);
            bill.Tovars.Add(5, 15);
            bills.Add(bill);*/
            billsCheckListBox.Items.Clear();
            billsCheckListBox.Items.AddRange(bills.ToArray());
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            int countBillsToSave = billsCheckListBox.CheckedItems.Count;
            for (int i = 0; i < countBillsToSave; i++)
            { 
                BillScanner bill = (BillScanner)billsCheckListBox.CheckedItems[i];
                BillScannerSerializator.Save(bill, getFilePathToBill(bill));
                saveProgress.Value = (i + 1) * 100 / countBillsToSave;
            }
            saveProgress.Value = 0;
            MessageBox.Show("Всі звіти збережено!");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            load();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            load();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            markAllAs(true);
        }

        private void markAllAs(bool mark)
        {
            int itemsCount = billsCheckListBox.Items.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                billsCheckListBox.SetItemChecked(i, mark);
            }
        }

        private void deselectBtn_Click(object sender, EventArgs e)
        {
            markAllAs(false);
        }

        private string getFilePathToBill(BillScanner bill) {
            string filename = string.Format(fileNameTemplate, bill.ClientID.ToString() + bill.OutDate.Year.ToString() + bill.OutDate.Month.ToString() + bill.OutDate.Day.ToString());
            string path = ConfigurationManager.AppSettings["billsFolderPath"];
            return path + @"\" + filename;
        }
    }
}
