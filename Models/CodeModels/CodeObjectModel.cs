using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMLGenerator.Models.CodeModels
{
    public class CodeObjectModel : BaseModel
    {
        #region Properties
        public string Name { get; set; }
        public Dictionary<string, string> FieldsFound { get; set; } = new Dictionary<string, string>();
        public CodeComponentTypeModel Type { get; set; }
        public ObservableCollection<CodeObjectModel> Children { get; set; } = new ObservableCollection<CodeObjectModel>();
        #endregion
    }
}
