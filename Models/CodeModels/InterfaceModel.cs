using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public class InterfaceModel : BaseObjectCodeModel, ICodeHasBases
    {
        #region Properties
        public List<string> Bases { get; set; }

        public override string NamePattern => @"(^| +)interface +(?<Name>((\w+ *<[^>]+>)|\w+))";
        #endregion

        #region Static Fields
        public static string BasePattern = @"(^| +)interface ";
        #endregion

        #region Constructors
        public InterfaceModel(string statement, string path, string nameSpace) : base(statement, path, nameSpace)
        {
        }

        public override string TransferToUML(int layer, Dictionary<string, List<string>> classesDict, Dictionary<string, List<string>> interfacesDict)
        {
            if(IsChecked == true)
            {
                string tab = String.Concat(System.Linq.Enumerable.Repeat("\t", layer));
                string output = $"{tab}{ViewModels.Main.UMLViewModel.AccessModifiersDict[AccessModifier]}interface {Path}{Name} " + "{\n";

                var methods = Children.OfType<MethodModel>();
                if (methods.Count() > 0)
                {
                    output += $"{tab}.. Methods ..\n";
                    foreach (var model in methods)
                    {
                        output += model.TransferToUML(layer + 1, classesDict, interfacesDict);
                    }
                }
                var properties = Children.OfType<PropertyModel>();
                if (properties.Count() > 0)
                {
                    output += $"{tab}.. Properties ..\n";
                    foreach (var model in properties)
                    {
                        output += model.TransferToUML(layer + 1, classesDict, interfacesDict);
                    }
                }
                return output + tab + "}\n\n";
            }
            return "";
        }
        #endregion
    }
}
