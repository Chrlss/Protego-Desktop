using Protego_Desktop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protego_Desktop.Model;

namespace Protego_Desktop.ViewModel
{
    class ProtectionVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;
        public int CustomerID
        {
            get { return _pageModel.Protection; }
            set { _pageModel.Protection = value; OnPropertyChanged(); }
        }

        public ProtectionVM()
        {
            _pageModel = new PageModel();
            CustomerID = 100528;
        }
    }
}
