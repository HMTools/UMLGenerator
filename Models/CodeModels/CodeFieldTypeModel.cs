using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ObservableCollection<CodeCaseModel> Cases { get; set; } = new ObservableCollection<CodeCaseModel>();
        private FieldInputType inputType;

        public FieldInputType InputType
        {
            get { return inputType; }
            set { 
                inputType = value;
                NotifyPropertyChanged(); }
        }

        #endregion
    }

    #region Enums
    public enum FieldInputType
    {
        Textual,
        Boolean,
        Switch
    }
    #endregion

}
