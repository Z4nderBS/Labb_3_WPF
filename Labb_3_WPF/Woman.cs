using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Labb_3_WPF
{
    public class Woman : IBooking
    {
        public string name { get; set; }
        public string gender { get; set; }
        public DateOnly date { get; set; }
        public string time { get; set; }
        public string phoneNr { get; set; }
        public string table { get; set; }
        public string text { get; set; }
       

        
        public Woman(string name, DateOnly date, string time, string phoneNr, string table)
        {
            gender = "Kvinna";
            this.name = name;
            this.date = date;
            this.time = time;
            this.phoneNr = phoneNr;
            this.table = table;
            text = $"Bord: {table}. Klockan: {time}. Namn: {name}. Kön: {gender}. Telefonnummer: {phoneNr}. Datum: {date}.";


        }

    
    }
}
       
       
