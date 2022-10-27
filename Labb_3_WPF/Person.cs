using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_3_WPF
{
    public interface IPerson
    {
        string namn { get; set; }
        int age { get; set; }
        int phoneNumber { get; set; }
    }

}
