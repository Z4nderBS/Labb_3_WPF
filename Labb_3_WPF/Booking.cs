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
        public double time { get; set; }
        public int table { get; set; }
        public string text { get; set; }

        public Booking(string name, DateOnly date, double time, int table, string text)
        {
            this.name = name;
            this.date = date;
            this.time = time;
            this.table = table;
            this.text = text;
        }
    }
}
