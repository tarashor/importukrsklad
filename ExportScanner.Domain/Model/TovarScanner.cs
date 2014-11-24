using System;
namespace ExportScanner.Domain.Model
{
    [Serializable]
    public class TovarScanner
    {
        public string Name { get; set; }
        public string GroupName { get; set; }
        public int ID { get; set; }
    }
}
