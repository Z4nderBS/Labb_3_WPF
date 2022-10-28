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
        public string[] bordAvailable = new string[5];
       

        public Time(string namn)
        {
            this.tid = namn;
            bordAvailable[0] = " ";
            bordAvailable[1] = " ";
            bordAvailable[2] = " ";
            bordAvailable[3] = " ";
            bordAvailable[4] = " ";
        
        }


    }

}


        





