using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Wpf.Ui;

namespace Protego.ViewModel
{
    public class MyViewModel : INotifyPropertyChanged
    {
        
        private readonly INavigationService _navigationService;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand CardActionCommand { get; private set; }

        public MyViewModel(INavigationService navigationService)
        {
            CardActionCommand = new DelegateCommand(OnCardAction);
            _navigationService = navigationService;
        }

        private void OnCardAction()
        {
            _navigationService.NavigateTo("Monitor");
        }
        public interface INavigationService
        {
            void NavigateTo(string Monitor);
        }


    }
}
