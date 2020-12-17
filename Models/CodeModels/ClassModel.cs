using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public class ClassModel : BaseObjectCodeModel, ICodeAbstractable, ICodeHasBases
    {
        #region Properties
        public bool IsAbstract { get; set; }
        public List<string> Bases { get; set; }
        public List<ClassModel> Classes { get; set; }
        public List<InterfaceModel> Interfaces { get; set; }
        public List<MethodModel> Methods { get; set; }
        public List<PropertyModel> Properties { get; set; }
        public List<FieldModel> Fields { get; set; }

        public override string NamePattern => @"(^| +)class +(?<Name>((\w+ *<[^>]+>)|\w+))";
        #endregion

        #region Static Fields
        public static string BasePattern = @"(^| +)class ";
        #endregion

        #region Constructors
        public ClassModel(string statement, string path, string nameSpace) : base(statement, path, nameSpace)
        {
            Classes = new List<ClassModel>();
            Interfaces = new List<InterfaceModel>();
            Methods = new List<MethodModel>();
            Properties = new List<PropertyModel>();
            Fields = new List<FieldModel>();
        }
        #endregion

        #region Methods
        public override void AssociateChilds(List<object> childs)
        {
            foreach (var child in childs)
            {
                switch (child)
                {
                    case ClassModel obj:
                        Classes.Add(obj);
                        break;
                    case InterfaceModel obj:
                        Interfaces.Add(obj);
                        break;
                    case MethodModel obj:
                        Methods.Add(obj);
                        break;
                    case PropertyModel obj:
                        Properties.Add(obj);
                        break;
                    case FieldModel obj:
                        Fields.Add(obj);
                        break;
                }
            }
        }

        public override string TransferToUML(int layer, Dictionary<string, List<string>> classesDict, Dictionary<string, List<string>> interfacesDict)
        {
            string tab = String.Concat(System.Linq.Enumerable.Repeat("\t", layer));
            string output = $"{tab}{ViewModels.UMLScreenViewModel.AccessModifiersDict[AccessModifier]}class {Path}{Name} " + "{\n";
            if (Methods.Count > 0)
            {
                output += $"{tab}.. Methods ..\n";
                foreach (var model in Methods)
                {
                    output += model.TransferToUML(layer + 1, classesDict, interfacesDict);
                }
            }
            if(Properties.Count > 0)
            {
                output += $"{tab}.. Properties ..\n";
                foreach (var model in Properties)
                {
                    output += model.TransferToUML(layer + 1, classesDict, interfacesDict);
                }
            }
            if(Fields.Count > 0)
            {
                output += $"{tab}.. Fields ..\n";
                foreach (var model in Fields)
                {
                    output += model.TransferToUML(layer + 1, classesDict, interfacesDict);
                }
            }       
            return output + tab + "}\n\n";
        }
        #endregion
    }
}
