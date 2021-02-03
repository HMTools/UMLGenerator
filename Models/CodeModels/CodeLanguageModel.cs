using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UMLGenerator.Models.CodeModels
{
    public class CodeLanguageModel : CodeComponentTypeModel
    {
        #region Properties

        public Dictionary<string, CodeComponentTypeModel> Components { get; set; } = new Dictionary<string, CodeComponentTypeModel>();

        public ObservableCollection<CodeCleanupModel> CleanupModels { get; set; } = new ObservableCollection<CodeCleanupModel>();
        public string FileExtension { get; set; } = "";

        #endregion Properties
    }
}