using ExportScanner.Domain.Model;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
