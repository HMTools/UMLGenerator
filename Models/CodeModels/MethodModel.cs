using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public class MethodModel : IUMLTransferable
    {
        #region Properties
        public string Name { get; set; }
        public string ReturnType { get; set; }
        public string Parameters { get; set; }
        public string AccessModifier { get; set; }
        public bool IsAbstract { get; set; }
        #endregion

        #region Static Fields
        public static string BasePattern = @" ((\w+ *<.*>)|\w+)(?= *\()";
        #endregion

        #region Constructors
        public MethodModel(string statement)
        {
            var accessMatch = Regex.Match(statement, @"(^| +)(?<AcessModifier>public|(protected internal)|protected|internal|private|(private protected)) +");

            Name = Regex.Match(statement, @"(?<Name>(\w+ *<[^>]+>)|\w+) *\(").Groups["Name"].Value;
            ReturnType = Regex.Match(statement, @"(?<ReturnType>(\w+ *<.+>)|\w+) +((\w+ *<[^>]+>)|\w+) *\(").Groups["ReturnType"].Value;
            Parameters = Regex.Match(statement, @"\w+\((?<Parameters>.*)\)").Groups["Parameters"].Value;
            AccessModifier = accessMatch.Success ? accessMatch.Groups["AcessModifier"].Value : "";
            IsAbstract = Regex.Match(statement, @"(^| +)(abstract) +").Success;
        }

        #endregion

        #region Methods
        public string TransferToUML(int layer)
        {
            string tab = String.Concat(System.Linq.Enumerable.Repeat("\t", layer));
            return $"{tab}{ViewModels.UMLScreenViewModel.AccessModifiersDict[AccessModifier]}{ReturnType} {Name}({Parameters})\n\n";
        }

        #endregion
    }
}
