using ExportScanner.Domain.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ExportScanner.Domain.Utility
{
    public class BillScannerSerializator
    {
        public static BillScanner Load(string filename)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            BillScanner bill = (BillScanner)formatter.Deserialize(stream);
            stream.Close();
            return bill;
        }

        public static void Save(BillScanner bill, string filename)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, bill);
            stream.Close();
        }
    }
}
