using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public class RecordModel : BaseObjectCodeModel
    {
        #region Properties
        public override string NamePattern => @"(^| +)record +(?<Name>\w+)";

        #endregion

        #region Static Fields
        public static string BasePattern = @"(^| +)record +";
        #endregion

        #region Constructors
        public RecordModel(string statement, string path, string nameSpace) : base(statement, path, nameSpace)
        {
        }
        #endregion

        #region Methods
        public override void AssociateChilds(List<object> childs)
        {
            throw new NotImplementedException();
        }

        public override string TransferToUML(int layer, Dictionary<string, List<string>> classesDict, Dictionary<string, List<string>> interfacesDict)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
