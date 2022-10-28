using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_3_WPF
{
    public class CheckDateAndTime
    {
       
        public Time klockan_4 = new Time("16.00");
        public Time klockan_5 = new Time("17.00");
        public Time klockan_6 = new Time("18.00");
        public Time klockan_7 = new Time("19.00");
        public Time klockan_8 = new Time("20.00");
        public Time klockan_9 = new Time("21.00");

        public List<Time> Tider = new List<Time>();


        public DateOnly datum { get; set; }
        public bool fullbokad { get; set; }


        public CheckDateAndTime(DateOnly datum)
        {
            Tider.Add(klockan_4);
            Tider.Add(klockan_5);
            Tider.Add(klockan_6);
            Tider.Add(klockan_7);
            Tider.Add(klockan_8);
            Tider.Add(klockan_9);
         
            this.datum = datum;
        }
    }

    public class Time
    {
        public string tid { get; set; }
        public Table table1 = new Table(1);
        public Table table2 = new Table(2);
        public Table table3 = new Table(3);
        public Table table4 = new Table(4);
        public Table table5 = new Table(5);
        public Table[] tables = new Table[5];
       

        public Time(string namn)
        {
            this.tid = namn;
            table1.available = true;
            table2.available = true;
            table3.available = true;
            table4.available = true;
            table5.available = true;

            tables[0] = table1;
            tables[1] = table2;
            tables[2] = table3;
            tables[3] = table4;
            tables[4] = table5;
            
        
        }


    }

    public class Table
    {
        public bool available { get; set; }
        public int num { get; set; }

        public Table(int num)
        {
            this.num = num;
        }
    }

}


        





