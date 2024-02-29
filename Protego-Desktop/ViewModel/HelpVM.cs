using Protego_Desktop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Protego_Desktop.ViewModel
{
    class HelpVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;
        public decimal TransactionAmount
        {
            get { return _pageModel.Help; }
            set { _pageModel.Help = value; OnPropertyChanged(); }
        }

        public HelpVM()
        {
            _pageModel = new PageModel();
            TransactionAmount = 5638;
        }
    }
}
