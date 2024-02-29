using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protego_Desktop.Model;

namespace Protego_Desktop.ViewModel
{
    class MonitorVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;
        public DateOnly DisplayOrderDate
        {
            get { return _pageModel.Monitor; }
            set { _pageModel.Monitor = value; OnPropertyChanged(); }
        }

        public MonitorVM()
        {
            _pageModel = new PageModel();
            DisplayOrderDate = DateOnly.FromDateTime(DateTime.Now);
        }
    }
}
