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
        public string Name { get; set; }
        public string Pattern { get; set; }
        public ObservableCollection<string> Values { get; set; } = new ObservableCollection<string>();
        public FieldInputType InputType { get; set; }
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
