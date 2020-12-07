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
        public void AssociateChilds(List<object> childs)
        {
            foreach (var child in childs)
            {
                switch (child)
                {
                    case ClassModel obj:
                        Classes.Add(obj);
                        break;
                    case EnumModel obj:
                        Enums.Add(obj);
                        break;
                    case InterfaceModel obj:
                        Interfaces.Add(obj);
                        break;
                    case RecordModel obj:
                        Records.Add(obj);
                        break;
                }
            }
        }

        public string TransferToUML(int layer)
        {
            string tab = String.Concat(System.Linq.Enumerable.Repeat("\t", layer));
            string output = tab + "namespace " + Name + " {\n";
            foreach (var model in Classes)
            {
                if(model.Path != "")
                    output += $"\t{model.Path.Substring(0, model.Path.Length-1)} +-- {model.Path}{model.Name}\n";
                output += model.TransferToUML(layer+1);
            }
            foreach (var model in Enums)
            {
                output += model.TransferToUML(layer + 1);
            }
            foreach (var model in Interfaces)
            {
                output += model.TransferToUML(layer + 1);
            }
            foreach (var model in Records)
            {
                output += model.TransferToUML(layer + 1);
            }
            return  output + tab + "}\n";
        }
        #endregion
    }
}
