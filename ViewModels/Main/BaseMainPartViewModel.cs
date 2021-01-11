﻿using MVVMLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFLibrary.Commands;

namespace UMLGenerator.ViewModels.Main
{
    public class BaseMainPartViewModel : BaseViewModel
    {
        #region Commands

        public RelayCommand ToggleShowCommand { get; private set; }

        #endregion
        #region Properties
        private bool isShown = true;

        public bool IsShown
        {
            get { return isShown; }
            set { isShown = value; NotifyPropertyChanged(); NotifyPropertyChanged("ColMinWidth"); }
        }

        public int ColMinWidth
        {
            get { return IsShown ? 300 : 0; }
        }

        #endregion

        #region Fields
        protected readonly MainViewModel mainVM;
        #endregion

        #region Constructors
        public BaseMainPartViewModel(MainViewModel mainVM)
        {
            this.mainVM = mainVM;
        }
        #endregion

        #region Methods
        protected override void AddCommands()
        {
            base.AddCommands();
            ToggleShowCommand = new RelayCommand(o => IsShown = !IsShown);
        }
        #endregion
    }
}
