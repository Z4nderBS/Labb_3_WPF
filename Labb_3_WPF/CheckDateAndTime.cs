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
        public Time klocakn_7 = new Time("19.00");
        public Time klockan_8 = new Time("20.00");
        public Time klockan_9 = new Time("21.00");
       
        public DateOnly datum { get; set; }
        public bool fullbokad { get; set; }


        public CheckDateAndTime(DateOnly datum)
        {
 
            this.datum = datum;
        }


        public static string CheckTimeAvailable(Time tid)
        {
            string message = "";
            for (int i = 0; i < tid.bordAvailable.Length; i++)
            {
                if (tid.bordAvailable[i] == " ")
                {

                }
            }




            return message;
        }
    }

    public class Time
    {
        public string namn { get; set; }
        public string[] bordAvailable = new string[5];
       

        public Time(string namn)
        {
            this.namn = namn;
            bordAvailable[0] = " ";
            bordAvailable[1] = " ";
            bordAvailable[2] = " ";
            bordAvailable[3] = " ";
            bordAvailable[4] = " ";
        
        }


    }

}
