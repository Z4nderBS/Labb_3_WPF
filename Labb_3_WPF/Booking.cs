using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_3_WPF
{
    public class Booking
    {
        public string name { get; set; }
        public DateOnly date { get; set; }
        public string time { get; set; }
        public int phoneNr { get; set; }
        public string text { get; set; }
        public string kön { get; set; }

        public Booking(string name, string kön, DateOnly date, string time, int phoneNr, string text)
        {
            this.name = name;
            this.kön = kön;
            this.date = date;
            this.time = time;
            this.phoneNr = phoneNr;
            this.text = text;
           
        }
    }
}
