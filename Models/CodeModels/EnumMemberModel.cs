using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMLGenerator.Models.CodeModels
{
    public class EnumMemberModel : BaseCodeModel
    {
        public EnumMemberModel(string statement) : base(statement)
        {
        }

        public override string NamePattern => @"^ *(?<Name>\w+)";

        public override string TransferToUML(int layer, Dictionary<string, List<string>> classesDict, Dictionary<string, List<string>> interfacesDict)
        {
            string tab = String.Concat(System.Linq.Enumerable.Repeat("\t", layer));
            return $"{tab}{Name}\n";
        }
    }
}
