using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public class InterfaceModel : IUMLTransferable
    {
        #region Properties
        public string Name { get; set; }
        public string AccessModifier { get; set; }
        public string Path { get; set; }
        public string Namespace { get; set; }
        #endregion

        #region Static Fields
        public static string BasePattern = @"(^| +)interface +";
        #endregion

        #region Constructors
        public InterfaceModel(string statement, string path, string nameSpace)
        {
            Path = path;
            Namespace = nameSpace;

            var accessMatch = Regex.Match(statement, @"(^| +)(?<AcessModifier>public|(protected internal)|protected|internal|private|(private protected)) +");

            Name = Regex.Match(statement, @"(^| +)interface +(?<Name>((\w+ *<[^>]+>)|\w+))").Groups["Name"].Value;
            AccessModifier = accessMatch.Success ? accessMatch.Groups["AcessModifier"].Value : "";
        }

        public string TransferToUML(int layer)
        {
            string tab = String.Concat(System.Linq.Enumerable.Repeat("\t", layer));
            string output = tab + "interface " + Name + " {\n";
            return output + tab + "}\n\n";
        }
        #endregion
    }
}
