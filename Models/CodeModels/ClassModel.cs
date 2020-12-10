using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public class ClassModel : IUMLTransferable
    {
        #region Properties
        public string Name { get; set; }
        public string AccessModifier { get; set; }
        public bool IsAbstract { get; set; }
        public List<string> Bases { get; set; }
        public List<MethodModel> Methods { get; set; }
        public List<PropertyModel> Properties { get; set; }
        public List<FieldModel> Fields { get; set; }
        public string Path { get; set; }
        public string Namespace { get; set; }
        #endregion

        #region Static Fields
        public static string BasePattern = @"(^|\w* +)class ";
        #endregion

        #region Constructors
        public ClassModel(string statement, string path, string nameSpace)
        {
            Path = path;
            Namespace = nameSpace;
            var basesMatch = Regex.Match(statement, @":(?<Bases>.*){");

            Name = Regex.Match(statement, @"(^| +)class +(?<Name>((\w+ *<[^>]+>)|\w+))").Groups["Name"].Value;
            AccessModifier = Libraries.RegexPatterns.GetAccessModifier(statement);
            Bases = basesMatch.Success ? basesMatch.Groups["Bases"].Value.Split(',').ToList() :null;
            IsAbstract = Regex.IsMatch(statement, @"(^| +)(abstract) +");

            Methods = new List<MethodModel>();
            Properties = new List<PropertyModel>();
            Fields = new List<FieldModel>();
            Bases = new List<string>();
        }
        #endregion

        #region Methods
        public void AssociateChilds(List<object> childs)
        {
            foreach (var child in childs)
            {
                switch (child)
                {
                    case MethodModel obj:
                        Methods.Add(obj);
                        break;
                    case PropertyModel obj:
                        Properties.Add(obj);
                        break;
                    case FieldModel obj:
                        Fields.Add(obj);
                        break;
                }
            }
        }

        public string TransferToUML(int layer)
        {
            string tab = String.Concat(System.Linq.Enumerable.Repeat("\t", layer));
            string output = $"{tab}{ViewModels.UMLScreenViewModel.AccessModifiersDict[AccessModifier]}class {Path}{Name} " + "{\n";
            if (Methods.Count > 0)
            {
                output += $"{tab}.. Methods ..\n";
                foreach (var model in Methods)
                {
                    output += model.TransferToUML(layer + 1);
                }
            }
            if(Properties.Count > 0)
            {
                output += $"{tab}.. Properties ..\n";
                foreach (var model in Properties)
                {
                    output += model.TransferToUML(layer + 1);
                }
            }
            if(Fields.Count > 0)
            {
                output += $"{tab}.. Fields ..\n";
                foreach (var model in Fields)
                {
                    output += model.TransferToUML(layer + 1);
                }
            }       
            return output + tab + "}\n\n";
        }
        #endregion
    }
}
