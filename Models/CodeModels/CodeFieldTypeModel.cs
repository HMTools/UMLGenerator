using System.Collections.ObjectModel;

namespace UMLGenerator.Models.CodeModels
{
    public class CodeFieldTypeModel : BaseModel
    {
        #region Properties

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged(); }
        }

        public string Pattern { get; set; }
        public string TrueValue { get; set; }
        public string FalseValue { get; set; }
        private FieldInputType inputType = FieldInputType.Textual;
        public string PathAncestor { get; set; }
        public string PathField { get; set; }
        public ObservableCollection<CodeCaseModel> SwitchCases { get; set; } = new ObservableCollection<CodeCaseModel>();

        public FieldInputType InputType
        {
            get { return inputType; }
            set
            {
                inputType = value;
                NotifyPropertyChanged();
            }
        }

        #endregion Properties
    }

    #region Enums

    public enum FieldInputType
    {
        Textual,
        Boolean,
        Switch,
        Path
    }

    #endregion Enums
}