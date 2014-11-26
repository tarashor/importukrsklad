using ExportScanner.Domain;
using ExportScanner.Domain.Model;
using ExportScanner.Domain.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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

            billsCheckListBox.Items.Clear();
            billsCheckListBox.Items.AddRange(bills.ToArray());
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            logTxt.Text = string.Empty;
            int countBillsToSave = billsCheckListBox.CheckedItems.Count;
            for (int i = 0; i < countBillsToSave; i++)
            { 
                BillScanner bill = (BillScanner)billsCheckListBox.CheckedItems[i];
                log("Початок збереження звіту для " + bill.ClientName);
                List<string> files =  getFilePathsToBill(bill);
                foreach (string file in files)
                {
                    try
                    {
                        BillScannerSerializator.Save(bill, file);
                        log(@"   Звіт збережено у " + file);
                    }
                    catch (IOException ex)
                    {
                        log(@"   Неможливо зберегти звіт у " + file);
                    }
                }
                saveProgress.Value = (i + 1) * 100 / countBillsToSave;
            }
            saveProgress.Value = 0;
            log("Всі звіти збережено!");
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

        private const string SEPERATOR_IN_FILENAME = "_";
        private const char SEPERATOR_IN_PATHS = ';';

        private List<string> getFilePathsToBill(BillScanner bill)
        {
            string filename = string.Format(fileNameTemplate, bill.ClientName + SEPERATOR_IN_FILENAME + bill.OutDate.Year.ToString() + SEPERATOR_IN_FILENAME + bill.OutDate.Month.ToString() + SEPERATOR_IN_FILENAME + bill.OutDate.Day.ToString());
            string pathsString = ConfigurationManager.AppSettings["billsFolderPath"];
            string[] paths = pathsString.Split(SEPERATOR_IN_PATHS);
            List<string> files = new List<string>(paths.Length);
            for (int i = 0; i < paths.Length; i++)
            {
                files.Add(paths[i].Trim() + @"\" + filename);
            }

            return files;
        }

        private void log(string l) {
            logTxt.AppendText(l + "\r\n"); 
        }
    }
}
