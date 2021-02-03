using MVVMLibrary.ViewModels;
using UMLGenerator.Models.CodeModels;
using WPFLibrary.Commands;

namespace UMLGenerator.ViewModels.CodeLanguages
{
    public class CodeFieldViewModel : BaseExpandableViewModel
    {
        #region Commands

        public RelayCommand AddCaseCommand { get; private set; }
        public RelayCommand RemoveCaseCommand { get; private set; }

        #endregion Commands

        #region Properties

        public CodeFieldTypeModel Field { get; set; }

        #endregion Properties

        #region Constructors

        public CodeFieldViewModel(CodeFieldTypeModel field)
        {
            Field = field;
        }

        #endregion Constructors

        #region Methods

        protected override void AddCommands()
        {
            base.AddCommands();
            AddCaseCommand = new RelayCommand(o =>
            {
                Field.SwitchCases.Add(new CodeCaseModel());
            });
            RemoveCaseCommand = new RelayCommand(o =>
            {
                Field.SwitchCases.Remove(o as CodeCaseModel);
            });
        }

        #endregion Methods
    }
}