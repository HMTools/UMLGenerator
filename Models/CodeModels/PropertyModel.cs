using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public class PropertyModel : BaseCodeModel, ICodeAccessDeclared
    {
        #region Properties
        public string Type { get; set; }
        public string AccessModifier { get; set; }

        public override string NamePattern => @"(?<Name>\w+) *{";
        #endregion

        #region Public Static Field
        public static string BasePattern = @"\w+ *{";
        #endregion

        #region Constructors
        public PropertyModel(string statement) : base(statement)
        {
            Type = Regex.Match(statement, @"(?<Type>(\w+ *<.+>)|\w+) +\w+ *{").Groups["Type"].Value;
        }
        #endregion
        #region Methods
        public override string TransferToUML(int layer, Dictionary<string, List<string>> classesDict, Dictionary<string, List<string>> interfacesDict)
        {
            string tab = String.Concat(System.Linq.Enumerable.Repeat("\t", layer));
            return $"{tab}{ViewModels.Main.UMLViewModel.AccessModifiersDict[AccessModifier]}{Name} : {Type}\n\n";
        }
        #endregion
    }
}
