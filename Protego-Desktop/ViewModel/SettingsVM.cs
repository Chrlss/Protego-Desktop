using Protego_Desktop.Model;
using Protego_Desktop.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protego_Desktop.ViewModel
{
    class SettingsVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;

        public bool Settings { get; }

        public SettingsVM()
        {
            _pageModel = new PageModel();
            Settings = true;
        }
    }
}
