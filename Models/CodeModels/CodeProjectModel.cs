using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace UMLGenerator.Models.CodeModels
{
    public class CodeProjectModel : BaseModel
    {
        #region Properties

        public CodeLanguageModel Language { get; set; }

        //Collections[ComponentType][ComponentName][List Of Components With That Type And Name]]
        public Dictionary<string, Dictionary<string, List<CodeObjectModel>>> Collections { get; set; } = new Dictionary<string, Dictionary<string, List<CodeObjectModel>>>();

        public ObservableCollection<CodeObjectModel> Children { get; set; } = new ObservableCollection<CodeObjectModel>();

        #endregion Properties

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

            output = Regex.Replace(output, @"\[\[@Component\((.+)\)@\]\]", match =>
            {
                string ret = "";
                var typeName = match.Groups[1].Value.Trim();
                Children.Where(child => child.Type.Name == typeName).ToList()
                .ForEach(child => ret += $"{child.TransferToUML(0)}\n");
                return ret;
            });

            #endregion Replace Children Components (Pattern: [[|<ComponentType Name>|]])

            output = Regex.Replace(output, @"\[\[@Collection\((?<ComponentType>.+?),(?<ComponentName>.+?),(?<ComponentField>.+?)\)@\]\]", GetCollectionValue);
            return output + "@enduml";
        }

        #endregion Constructors

        private string GetCollectionValue(Match collectionQueryMatch)
        {
            //\[\[@Collection\((?<ComponentType>.+?),(?<ComponentName>.+?),(?<ComponentField>.+?)\)@\]\]
            string type = collectionQueryMatch.Groups["ComponentType"].Value;
            string name = collectionQueryMatch.Groups["ComponentName"].Value;
            string field = collectionQueryMatch.Groups["ComponentField"].Value;
            if (!Collections.ContainsKey(type) || !Collections[type].ContainsKey(name))
                return "";
            var firstMatchedComponent = Collections[type][name].First(comp => comp.FieldsFound.ContainsKey(field));
            if (firstMatchedComponent == null)
                return "";
            return firstMatchedComponent.FieldsFound[field];
        }
    }
}