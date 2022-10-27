using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_3_WPF
{
    public class Man : IPerson
    {
        public string gender { get; set; }
        public string namn { get; set; }
        public int age { get; set; }
        public int phoneNumber { get; set; }

        public Man(string namn, int age, int phoneNumber)
        {
            gender = "Man";
            this.namn = namn;
            this.age = age;
            this.phoneNumber = phoneNumber;
        }
    }
}
