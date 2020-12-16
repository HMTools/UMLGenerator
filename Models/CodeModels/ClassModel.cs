using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public class ClassModel : IUMLTransferable, ICodeObject
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
        public static string BasePattern = @"(^| +)class ";
        #endregion

        #region Constructors
        public ClassModel(string statement, string path, string nameSpace)
        {
            Path = path;
            Namespace = nameSpace;

            Name = Regex.Match(statement, @"(^| +)class +(?<Name>((\w+ *<[^>]+>)|\w+))").Groups["Name"].Value;
            AccessModifier = Libraries.RegexPatterns.GetAccessModifier(statement);
            IsAbstract = Regex.IsMatch(statement, @"(^| +)(abstract) +");

            Methods = new List<MethodModel>();
            Properties = new List<PropertyModel>();
            Fields = new List<FieldModel>();

            var basesMatch = Regex.Match(statement, @":(?<Bases>.*){");
            Bases = basesMatch.Success ? basesMatch.Groups["Bases"].Value.Split(',').ToList() :new List<string>();
            for(int i = 0; i < Bases.Count; i++)
            {
                Bases[i] = Bases[i].Trim();
            }
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

        public string TransferToUML(int layer, Dictionary<string, List<string>> classesDict, Dictionary<string, List<string>> interfacesDict)
        {
            string tab = String.Concat(System.Linq.Enumerable.Repeat("\t", layer));
            string output = $"{tab}{ViewModels.UMLScreenViewModel.AccessModifiersDict[AccessModifier]}class {Path}{Name} " + "{\n";
            if (Methods.Count > 0)
            {
                output += $"{tab}.. Methods ..\n";
                foreach (var model in Methods)
                {
                    output += model.TransferToUML(layer + 1, classesDict, interfacesDict);
                }
            }
            if(Properties.Count > 0)
            {
                output += $"{tab}.. Properties ..\n";
                foreach (var model in Properties)
                {
                    output += model.TransferToUML(layer + 1, classesDict, interfacesDict);
                }
            }
            if(Fields.Count > 0)
            {
                output += $"{tab}.. Fields ..\n";
                foreach (var model in Fields)
                {
                    output += model.TransferToUML(layer + 1, classesDict, interfacesDict);
                }
            }       
            return output + tab + "}\n\n";
        }
        #endregion
    }
}
