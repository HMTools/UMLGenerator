using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public class EnumModel : BaseObjectCodeModel
    {
        #region Properties
        public override string NamePattern => @"(^| +)enum +(?<Name>\w+)";
        #endregion

        #region Static Fields
        public static string BasePattern = @"(^| +)enum +";
        #endregion

        #region Constructors
        public EnumModel(string statement, string path, string nameSpace) : base(statement, path, nameSpace)
        {
        }

        public override string TransferToUML(int layer, Dictionary<string, List<string>> classesDict, Dictionary<string, List<string>> interfacesDict)
        {
            string tab = String.Concat(System.Linq.Enumerable.Repeat("\t", layer));
            string output = tab + "enum " + Name + " {\n";

            var members = Children.OfType<EnumMemberModel>();
            foreach (var member in members)
            {
                output += member.TransferToUML(layer+1, classesDict, interfacesDict);
            }
            return output + tab + "}\n\n";
        }
        #endregion
    }
}
