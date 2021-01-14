using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMLGenerator.Models.CodeModels
{
    public class CodeLanguageModel : CodeComponentTypeModel
    {
        #region Properties
        public string Name { get; set; }
        public Dictionary<string, CodeComponentTypeModel> Components { get; set; } = new Dictionary<string, CodeComponentTypeModel>();

        public ObservableCollection<CodeCleanupModel> CleanupModels { get; set; } = new ObservableCollection<CodeCleanupModel>();
        public ObservableCollection<CodeFieldTypeModel> CommonFields { get; set; } = new ObservableCollection<CodeFieldTypeModel>();
        public ObservableCollection<CodeDelimiterModel> CodeDelimiters { get; set; } = new ObservableCollection<CodeDelimiterModel>();
        #endregion
    }
}
