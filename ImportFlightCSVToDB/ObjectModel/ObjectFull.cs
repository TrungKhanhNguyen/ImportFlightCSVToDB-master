using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportFlightCSVToDB.ObjectModel
{
    public class ObjectFull
    {
        public string ICAO { get; set; }
        public DateTime dategenerate { get; set; }
        
        public DateTime datelog { get; set; }
        public string callsign { get; set; }
        public string altitude { get; set; }
        public string speed { get; set; }
        public string track { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string verticalrate { get; set; }
        public string squawk { get; set; }

        public string messageType { get; set; }
        public string transmissionType { get; set; }
    }
}
