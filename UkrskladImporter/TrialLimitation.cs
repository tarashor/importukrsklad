using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UkrskladImporter
{
    public class TrialLimitation
    {
        private static string subKey = "SOFTWARE\\" + Application.ProductName.ToUpper();
        private static string key = "counter";

        public static int GetCounter()
        {
            int res = 0;
            // Opening the registry key
            RegistryKey rk = Registry.LocalMachine;
            // Open a subKey as read-only
            RegistryKey sk1 = rk.OpenSubKey(subKey);
            // If the RegistrySubKey doesn't exist -> (null)
            if (sk1 != null)
            {
                try
                {
                    // If the RegistryKey exists I get its value
                    // or null is returned.
                    res = (int)sk1.GetValue(key.ToUpper(), 0);
                }
                catch (Exception e) { }
            }

            return res;
        }

<<<<<<< HEAD
        public const int MaxCounter = 10;
=======
        public const int MaxCounter = 15;
>>>>>>> f404d37f191b32f43d76573eb720478cf5d44d64

        public static void SaveCounter(int savesCount)
        {
            try
            {
                // Setting
                RegistryKey rk = Registry.LocalMachine;
                // I have to use CreateSubKey 
                // (create or open it if already exits), 
                // 'cause OpenSubKey open a subKey as read-only
                RegistryKey sk1 = rk.CreateSubKey(subKey);
                // Save the value
                sk1.SetValue(key.ToUpper(), savesCount);
            }
            catch (Exception e) { }
        }
    }
}
