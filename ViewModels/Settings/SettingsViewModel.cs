using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMLGenerator.ViewModels.Settings
{
    public class SettingsViewModel : BaseViewModel
    {

        #region Properties
        private BaseViewModel selectedViewModel;

        public BaseViewModel SelectedViewModel
        {
            get { return selectedViewModel; }
            set { selectedViewModel = value; NotifyPropertyChanged(); }
        }
        #endregion

        #region Fields
        private readonly MainViewModel mainVM;
        #endregion

        #region Constructors
        public SettingsViewModel(MainViewModel mainVM)
        {
            this.mainVM = mainVM;
            SelectedViewModel = new GithubSettingsViewModel(mainVM);
        }
        #endregion
    }
}
