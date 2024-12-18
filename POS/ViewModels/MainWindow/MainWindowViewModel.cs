using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using POS.Utilities;
using POS.Utilities.RelayCommands;

namespace POS.ViewModels.MainWindow
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Action TurnOffApplicationAction;
        public ICommand TurnOffApplicationCommand { get; }

        public MainWindowViewModel()
        {
            TurnOffApplicationCommand = new RelayCommand(TurnOffApplication);
        }

        public void TurnOffApplication(object obj)
        {
            TurnOffApplicationAction?.Invoke();
        }
    }
}
