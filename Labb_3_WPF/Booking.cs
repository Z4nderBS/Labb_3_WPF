using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Labb_3_WPF
{
    public interface IBooking
    {
        public string name { get; set; }
        public DateOnly date { get; set; }
        public string time { get; set; }
        public string phoneNr { get; set; }
        public string gender { get; set; }
        public string table { get; set; }
        public string text { get; set; }


    }

}

