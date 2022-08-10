using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSeller.ViewModels
{
    public class ShellViewModel: Conductor<object>
    {
        public void BuyBook_Button()
        {
            ActivateItemAsync(new BuyBookViewModel());
        }
    }
}
