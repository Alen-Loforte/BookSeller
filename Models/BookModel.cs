using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSeller.Models
{
    public class BookModel
    {
        public string BookTitle { get; set; }
        public string BookPrice { get; set; }
        public string BookUPC { get; set; }
        public string BookStock { get; set; }
    }
}
