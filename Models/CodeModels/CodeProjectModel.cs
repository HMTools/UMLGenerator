using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMLGenerator.Models.CodeModels
{
    public class CodeProjectModel : BaseModel
    {
        #region Properties
        public CodeLanguageModel Language { get; set; }
        public Dictionary<string, Dictionary<string, CodeObjectModel>> UniqueCollections { get; set; } = new Dictionary<string, Dictionary<string, CodeObjectModel>>();
        public ObservableCollection<CodeObjectModel> Children { get; set; } = new ObservableCollection<CodeObjectModel>();
        #endregion
        #region Constructors
        public CodeProjectModel(CodeLanguageModel language)
        {
            Language = language;
        }
        #endregion
    }
}
