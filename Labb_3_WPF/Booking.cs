using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Labb_3_WPF
{
    public class Booking
    {
        public string name { get; set; }
        public DateOnly date { get; set; }
        public string time { get; set; }
        public string phoneNr { get; set; }
        public string text { get; set; }
        public string gender { get; set; }

        public string table { get; set; }

        public Booking(string name, string gender, DateOnly date, string time, string phoneNr, string text, string table)
        {
            this.name = name;
            this.gender = gender;
            this.date = date;
            this.time = time;
            this.phoneNr = phoneNr;
            this.text = text;
            this.table = table;

            string BookingText()
            {
                return $"Bord: {bord}. Klockan: {time}. Namn: {name}. Kön: {gender}. Telefonnummer: {phoneNr}. Datum: {date}.";
            }
        }
      
    }
}
