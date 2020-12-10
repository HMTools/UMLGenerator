using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;
using UMLGenerator.Models.CodeModels;

namespace UMLGenerator.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Properties
        private BaseViewModel selectedViewModel;

        public BaseViewModel SelectedViewModel
        {
            get { return selectedViewModel; }
            set { selectedViewModel = value; NotifyPropertyChanged(); }
        }
        #endregion

        #region Constructors
        public MainViewModel()
        {
            SelectedViewModel = new SelectSourceViewModel(this);
        }
        #endregion

        #region Private Methods

        //private string GenerateUML(IEnumerable<IUMLTransferable> source)
        //{
        //    string res = "@startuml\n";
        //    foreach(var obj in source)
        //    {
        //        res += obj.TransferToUML(0);
        //    }
        //    return res + "@enduml";
        //}
        #endregion
    }
}
