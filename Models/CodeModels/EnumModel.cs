using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public class EnumModel : IUMLTransferable
    {
        #region Properties
        public string Name { get; set; }
        public string AccessModifier { get; set; }
        public string Path { get; set; }
        public string Namespace { get; set; }
        public List<string> Members { get; set; }
        #endregion

        #region Static Fields
        public static string BasePattern = @"(^| +)enum +";
        #endregion

        #region Constructors
        public EnumModel(string statement, string path, string nameSpace)
        {
            Members = new List<string>();

            Path = path;
            Namespace = nameSpace;

            var accessMatch = Regex.Match(statement, @"(^| +)(?<AcessModifier>public|(protected internal)|protected|internal|private|(private protected)) +");

            Name = Regex.Match(statement, @"(^| +)enum +(?<Name>\w+)").Groups["Name"].Value;
            AccessModifier = accessMatch.Success ? accessMatch.Groups["AcessModifier"].Value : "";
        }

        public string TransferToUML(int layer, Dictionary<string, List<string>> classesDict, Dictionary<string, List<string>> interfacesDict)
        {
            string tab = String.Concat(System.Linq.Enumerable.Repeat("\t", layer));
            string output = tab + "enum " + Name + " {\n";
            foreach(var member in Members)
            {
                output += $"{tab}\t{member}\n";
            }
            return output + tab + "}\n\n";
        }
        #endregion
    }
}
