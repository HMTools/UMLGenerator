using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public class NamespaceModel : IUMLTransferable
    {
        #region Properties
        public string Name { get; set; }
        public List<ClassModel> Classes { get; set; }
        public List<EnumModel> Enums { get; set; }
        public List<InterfaceModel> Interfaces { get; set; }
        public List<RecordModel> Records { get; set; }

        #endregion

        #region Static Fields
        public static string BasePattern = @"(^| +)namespace +([\w.]+) *{";
        #endregion

        #region Fields

        #endregion

        #region Constructors
        public NamespaceModel(string statement)
        {
            Name = Regex.Match(statement, @"(^| +)namespace +(?<Name>[\w.]+) *{").Groups["Name"].Value;
            Classes = new List<ClassModel>();
            Enums = new List<EnumModel>();
            Interfaces = new List<InterfaceModel>();
            Records = new List<RecordModel>();
        }
        #endregion

        #region Methods
        

        public string TransferToUML(int layer, Dictionary<string, List<string>> classesDict, Dictionary<string, List<string>> interfacesDict)
        {
            string tab = String.Concat(System.Linq.Enumerable.Repeat("\t", layer));
            string output = tab + "namespace " + Name + " {\n";
            foreach (var model in Classes)
            {
                for(int i = 0; i < model.Bases.Count; i++)
                {
                    if(i == 0 && classesDict.ContainsKey(model.Bases[i]))
                    {
                        output += $"\tclass {model.Name} extends {classesDict[model.Bases[i]][0]}{model.Bases[i]}\n"; // need to handle ambiguity!!!

                    }
                    else
                    {
                        if(interfacesDict.ContainsKey(model.Bases[i]))
                        {
                            output += $"\tclass {model.Name} implements {interfacesDict[model.Bases[i]][0]}{model.Bases[i]}\n"; // need to handle ambiguity!!!
                        }
                        else
                        {
                            string impOrExt = model.Bases[i].Length == 1 || model.Bases[i][0] != 'I' || !Char.IsUpper(model.Bases[i][1]) ? "extends" : "implements";
                            output += $"\tclass {model.Name} {impOrExt} ___Common___.{model.Bases[i]}\n";
                        }
                        
                    }
                }
                if(model.Path != "")
                    output += $"\t{model.Path.Substring(0, model.Path.Length-1)} +-- {model.Path}{model.Name}\n";
                output += model.TransferToUML(layer+1, classesDict, interfacesDict);
            }
            foreach (var model in Enums)
            {
                output += model.TransferToUML(layer + 1, classesDict, interfacesDict);
            }
            foreach (var model in Interfaces)
            {
                output += model.TransferToUML(layer + 1, classesDict, interfacesDict);
            }
            foreach (var model in Records)
            {
                output += model.TransferToUML(layer + 1, classesDict, interfacesDict);
            }
            return  output + tab + "}\n";
        }
        #endregion
    }
}
