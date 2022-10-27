using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Labb_3_WPF
{
    public class Kvinna : IPerson
    {
        public string gender { get; set; }
        public string namn { get; set; }
        public int age { get; set; }
        public int phoneNumber { get; set; }

        public Man(string namn, int age, int phoneNumber)
        {
            gender = "Kvinna";
            this.namn = namn;
            this.age = age;
            this.phoneNumber = phoneNumber;
        }
    }
}
