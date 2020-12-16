using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public class FieldModel : IUMLTransferable
    {
        #region Properties
        public string Name { get; set; }
        public string Type { get; set; }
        public string AccessModifier { get; set; }
        #endregion

        #region Public Static Field
        public static string BasePattern = @"\w+.*;";
        #endregion

        #region Constructors
        public FieldModel(string statement)
        {
            var accessMatch = Regex.Match(statement, @"(^| +)(?<AcessModifier>public|(protected internal)|protected|internal|private|(private protected)) +");

            if(Regex.IsMatch(statement, @"=")) // default value declared
            {
                Name = Regex.Match(statement, @"(?<Name>\w+) +=").Groups["Name"].Value;
                Type = Regex.Match(statement, @"(?<Type>((\w+ *<.+>)|\w+)) +\w+ +=").Groups["Type"].Value;
            }
            else
            {
                Name = Regex.Match(statement, @"(?<Name>\w+) *;").Groups["Name"].Value;
                Type = Regex.Match(statement, @"(?<Type>((\w+ *<.+>)|\w+)) +\w+ *;").Groups["Type"].Value;
            }
            AccessModifier = accessMatch.Success ? accessMatch.Groups["AcessModifier"].Value : "";
        }
        #endregion

        #region Methods
        public string TransferToUML(int layer, Dictionary<string, List<string>> classesDict, Dictionary<string, List<string>> interfacesDict)
        {
            string tab = String.Concat(System.Linq.Enumerable.Repeat("\t", layer));
            return $"{tab}{ViewModels.UMLScreenViewModel.AccessModifiersDict[AccessModifier]}{Name} : {Type}\n\n";
        }
        #endregion
    }
}
