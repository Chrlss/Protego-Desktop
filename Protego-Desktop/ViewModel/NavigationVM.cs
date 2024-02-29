using Protego_Desktop.Utilities;
using Protego_Desktop.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Input;


namespace Protego_Desktop.ViewModel
{
    class NavigationVM : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ICommand HomeCommand { get; set; }
        public ICommand ProtectionCommand { get; set; }
        public ICommand SecurityCommand { get; set; }
        public ICommand MonitorCommand { get; set; }
        public ICommand HelpCommand { get; set; }
        
        public ICommand SettingsCommand { get; set; }

        private void Home(object obj) => CurrentView = new HomeVM();
        private void Protection(object obj) => CurrentView = new ProtectionVM();
        private void Security(object obj) => CurrentView = new SecurityVM();
        private void Monitor(object obj) => CurrentView = new MonitorVM();
        private void Help(object obj) => CurrentView = new HelpVM();
        
        private void Settings(object obj) => CurrentView = new SettingsVM();

        public NavigationVM()
        {
            HomeCommand = new RelayCommand(Home);
            ProtectionCommand = new RelayCommand(Protection);
            SecurityCommand = new RelayCommand(Security);
            MonitorCommand = new RelayCommand(Monitor);
            HelpCommand = new RelayCommand(Help);
            
            SettingsCommand = new RelayCommand(Settings);

            // Startup Page
            CurrentView = new HomeVM();
        }
    }
}
