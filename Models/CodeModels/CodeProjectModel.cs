using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public string TransferToUML()
        {
            string output = "@startuml" + Environment.NewLine;
            output += Language.UMLPattern;
            #region Replace Children Components (Pattern: [[|<ComponentType Name>|]])
            output = Regex.Replace(output, @"\[\[\|(.+)\|\]\]", match =>
            {
                string ret = "";
                var typeName = match.Groups[1].Value.Trim();
                Children.Where(child => child.Type.Name == typeName).ToList()
                .ForEach(child => ret += $"{child.TransferToUML(0)}\n");
                return ret;
            });
            #endregion
            return output + "@enduml";
        }
        #endregion
    }
}
