﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImportUkrSklad.Ukrsklad.Domain
{
    public class UkrSkladTovar
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string MeasurementUnits { get; set; }
        public decimal Cina { get; set; }
        public decimal CinaRozdrib { get; set; } 
        public decimal CinaOptova { get; set; }
        public decimal Cina1 { get; set; } 
        public decimal Cina2 { get; set; }
        public decimal Cina3 { get; set; }
    }
}