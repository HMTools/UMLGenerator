using MVVMLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMLGenerator.Models.CodeModels;
using WPFLibrary;
using WPFLibrary.Commands;

namespace UMLGenerator.ViewModels.CodeLanguages
{
    public class CodeFieldViewModel : BaseExpandableViewModel
    {
        #region Commands

        public RelayCommand AddCaseCommand { get; private set; }
        public RelayCommand RemoveCaseCommand { get; private set; }

        #endregion

        #region Properties
        public CodeFieldTypeModel Field { get; set; }

        #endregion

        #region Constructors
        public CodeFieldViewModel(CodeFieldTypeModel field)
        {
            Field = field;
        }
        #endregion

        #region Methods
        protected override void AddCommands()
        {
            base.AddCommands();
            AddCaseCommand = new RelayCommand(o => 
            { 
                Field.SwitchCases.Add(new CodeCaseModel());
            });
            RemoveCaseCommand = new RelayCommand(_case => 
            {
                    Field.SwitchCases.Remove(_case as CodeCaseModel);
            });
        }
        #endregion
    }
}
