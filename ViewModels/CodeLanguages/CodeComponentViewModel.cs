using MVVMLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMLGenerator.Models.CodeModels;
using WPFLibrary.Commands;

namespace UMLGenerator.ViewModels.CodeLanguages
{
    public class CodeComponentViewModel : BaseViewModel
    {
        #region Commands

        public RelayCommand AddTruePatternCommand { get; private set; }
        public RelayCommand AddFalsePatternCommand { get; private set; }
        public RelayCommand RemoveTruePatternCommand { get; private set; }
        public RelayCommand RemoveFalsePatternCommand { get; private set; }

        #endregion
        #region Properties
        public CodeComponentTypeModel Component { get; set; }

        #endregion

        #region Constructors
        public CodeComponentViewModel(CodeComponentTypeModel component)
        {
            Component = component;
        }
        #endregion

        #region Methods
        protected override void AddCommands()
        {
            base.AddCommands();
            AddTruePatternCommand = new RelayCommand(o => Component.TruePatterns.Add(""));
            AddFalsePatternCommand = new RelayCommand(o => Component.FalsePatterns.Add(""));
            RemoveTruePatternCommand = new RelayCommand(o =>  Component.TruePatterns.RemoveAt((int)o));
            RemoveFalsePatternCommand = new RelayCommand(o => Component.FalsePatterns.RemoveAt((int)o));
        }
        #endregion
    }
}
