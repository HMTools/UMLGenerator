using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public class MethodModel : BaseCodeModel, ICodeAccessDeclared, ICodeAbstractable
    {
        #region Properties
        public string ReturnType { get; set; }
        public string Parameters { get; set; }
        public string AccessModifier { get; set; }
        public bool IsAbstract { get; set; }

        public override string NamePattern => @"(?<Name>(\w+ *<[^>]+>)|\w+) *\(";
        #endregion

        #region Static Fields
        public static string BasePattern = @" ((\w+ *<.*>)|\w+)(?= *\()";
        public static string BaseFalsePattern = @" new ";
        #endregion

        #region Constructors
        public MethodModel(string statement) : base(statement)
        {

            ReturnType = Regex.Match(statement, @"(?<ReturnType>(\w+ *<.+>)|\w+) +((\w+ *<[^>]+>)|\w+) *\(").Groups["ReturnType"].Value;
            Parameters = Regex.Match(statement, @"\w+\((?<Parameters>.*)\)").Groups["Parameters"].Value;
        }

        #endregion

        #region Methods
        public override string TransferToUML(int layer, Dictionary<string, List<string>> classesDict, Dictionary<string, List<string>> interfacesDict)
        {
            string tab = String.Concat(System.Linq.Enumerable.Repeat("\t", layer));
            return $"{tab}{ViewModels.Main.UMLViewModel.AccessModifiersDict[AccessModifier]}{ReturnType} {Name}({Parameters})\n\n";
        }

        #endregion
    }
}
