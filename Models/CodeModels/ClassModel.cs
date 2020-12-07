using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public class ClassModel : IUMLTransferable
    {
        #region Properties
        public string Name { get; set; }
        public string AccessModifier { get; set; }
        public bool IsAbstract { get; set; }
        public string BaseClass { get; set; }
        public List<MethodModel> Methods { get; set; }
        public List<ClassModel> Classes { get; set; }
        public List<PropertyModel> Properties { get; set; }
        public List<FieldModel> Fields { get; set; }
        public List<EnumModel> Enums { get; set; }
        public List<InterfaceModel> Interfaces { get; set; }
        public List<RecordModel> Records { get; set; }
        public string Path { get; set; }
        #endregion

        #region Static Fields
        public static string BasePattern = @"(^|\w* +)class ";
        #endregion

        #region Constructors
        public ClassModel(string statement, string path)
        {
            Path = path;
            var accessMatch = Regex.Match(statement, @"(^| +)(?<AcessModifier>public|(protected internal)|protected|internal|private|(private protected)) +");
            var basesMatch = Regex.Match(statement, @":(?<Bases>.*){");


            Name = Regex.Match(statement, @"class +(?<Name>\w+).*{").Groups["Name"].Value;
            AccessModifier = accessMatch.Success ? accessMatch.Groups["AcessModifier"].Value : "";
            BaseClass = basesMatch.Success ? basesMatch.Groups["Bases"].Value : "";
            IsAbstract = Regex.IsMatch(statement, @"(^| +)(abstract) +");

            Methods = new List<MethodModel>();
            Classes = new List<ClassModel>();
            Properties = new List<PropertyModel>();
            Fields = new List<FieldModel>();
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
                    case MethodModel obj:
                        Methods.Add(obj);
                        break;
                    case PropertyModel obj:
                        Properties.Add(obj);
                        break;
                    case FieldModel obj:
                        Fields.Add(obj);
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
            string output = $"{tab}{ViewModels.MainViewModel.AccessModifiersDict[AccessModifier]}class {Path}{Name} " + "{\n";
            if (Classes.Count > 0)
            {
                foreach (var model in Classes)
                {
                    //output += $"{tab}{Path}{Name} +-- {Path}{Name}.{model.Name}\n";
                }
            }
            if (Methods.Count > 0)
            {
                output += $"{tab}.. Methods ..\n";
                foreach (var model in Methods)
                {
                    output += model.TransferToUML(layer + 1);
                }
            }
            if(Properties.Count > 0)
            {
                output += $"{tab}.. Properties ..\n";
                foreach (var model in Properties)
                {
                    output += model.TransferToUML(layer + 1);
                }
            }
            if(Fields.Count > 0)
            {
                output += $"{tab}.. Fields ..\n";
                foreach (var model in Fields)
                {
                    output += model.TransferToUML(layer + 1);
                }
            }
            if(Enums.Count > 0)
            {
                output += $"{tab}.. Enums ..\n";
                foreach (var model in Enums)
                {
                    output += $"{tab}{Path}{Name} +-- {Path}{Name}.{model.Name}\n";
                    //output += model.TransferToUML(layer + 1);
                }
            }
            if(Interfaces.Count > 0)
            {
                output += $"{tab}.. Interfaces ..\n";
                foreach (var model in Interfaces)
                {
                    output += $"{tab}{Path}{Name} +-- {Path}{Name}.{model.Name}\n";
                    //output += model.TransferToUML(layer + 1);
                }
            }
            
            if(Records.Count > 0)
            {
                output += $"{tab}.. Records ..\n";
                foreach (var model in Records)
                {
                    output += $"{tab}{Path}{Name} +-- {Path}{Name}.{model.Name}\n";
                    //output += model.TransferToUML(layer + 1);
                }
            }
           
            return output + tab + "}\n\n";
        }
        #endregion
    }
}
