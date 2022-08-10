using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookSeller.Models
{
    public class MiscFunctions
    {
        public MiscFunctions() { }
        public static void ErrorBox(string msg)
        {
            MessageBox.Show(msg);
        }
    }
}
