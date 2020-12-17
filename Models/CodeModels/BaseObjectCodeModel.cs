using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public abstract class BaseObjectCodeModel : BaseCodeModel, ICodeObject, ICodeAccessDeclared
    {
        #region Properties
        public string Path { get; set; }
        public string Namespace { get; set; }
        public string AccessModifier { get; set; }
        public List<BaseCodeModel> Children { get; set; }

        #endregion

        #region Constructors
        public BaseObjectCodeModel(string statement, string path, string nameSpace) : base(statement)
        {
            Children = new List<BaseCodeModel>();

            Path = path;
            Namespace = nameSpace;
        }
        #endregion
    }
}
