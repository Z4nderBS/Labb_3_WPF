using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_3_WPF
{
    public class Man : IBooking
    {
        public string name { get; set; }
        public string gender { get; set; }
        public DateOnly date { get; set; }
        public string time { get; set; }
        public string phoneNr { get; set; }
        public string table { get; set; }
        public string text { get; set; }


        public Man(string name, DateOnly date, string time, string phoneNr, string table)
        {
            gender = "Man";
            this.name = name;
            this.date = date;
            this.time = time;
            this.phoneNr = phoneNr;
            this.table = table;
            text = $"Bord: {table}. Klockan: {time}. Namn: {name}. Kön: {gender}. Telefonnummer: {phoneNr}. Datum: {date}.";


        }
    }
}
            
