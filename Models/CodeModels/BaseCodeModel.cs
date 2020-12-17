using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public abstract class BaseCodeModel : IUMLTransferable
    {
        #region Properties
        public string Name { get; set; }
        public abstract string NamePattern { get; }
        //public abstract string BasePattern { get; }
        //public abstract string BaseFalsePattern { get; }

        #endregion

        #region Constructors
        public BaseCodeModel(string statement)
        {
            Name = Regex.Match(statement, NamePattern).Groups["Name"].Value;
            if(this is ICodeAbstractable)
                (this as ICodeAbstractable).IsAbstract = Regex.IsMatch(statement, @"(^| +)(abstract) +");
            if (this is ICodeAccessDeclared)
                (this as ICodeAccessDeclared).AccessModifier = Libraries.RegexPatterns.GetAccessModifier(statement);
            if(this is ICodeHasBases)
            {
                var basesMatch = Regex.Match(statement, @":(?<Bases>.*){");
                var model = this as ICodeHasBases;
                model.Bases = basesMatch.Success ? basesMatch.Groups["Bases"].Value.Split(',').ToList() : new List<string>();
                for (int i = 0; i < model.Bases.Count; i++)
                {
                    model.Bases[i] = model.Bases[i].Trim();
                }
            }
        }
        #endregion

        #region Methods
        //public static bool ValidatePatterns(string statement)
        //{
        //    if(Regex.IsMatch(statement, ))
        //}
        #endregion

        #region Abstract Methods
        public abstract string TransferToUML(int layer, Dictionary<string, List<string>> classesDict, Dictionary<string, List<string>> interfacesDict);
        #endregion
    }
}
