using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_3_WPF
{
    public class Booking
    {
        string name { get; set; }
        DateTime date { get; set; }
        decimal time { get; set; }
        int table { get; set; }

        public Booking(string name, DateTime date, decimal time, int table)
        {
            this.name = name;
            this.date = date;
            this.time = time;
            this.table = table;
        }
    }
}
