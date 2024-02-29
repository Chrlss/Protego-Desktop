using Protego_Desktop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protego_Desktop.Model;

namespace Protego_Desktop.ViewModel
{
    class SecurityVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;
        public string ProductAvailability
        {
            get { return _pageModel.Security; }
            set { _pageModel.Security = value; OnPropertyChanged(); }
        }

        public SecurityVM()
        {
            _pageModel = new PageModel();
            ProductAvailability = "Out of Stock";
        }
    }
}
