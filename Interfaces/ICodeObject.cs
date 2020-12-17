using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMLGenerator.Models.CodeModels;

namespace UMLGenerator.Interfaces
{
    public interface ICodeObject
    {
        public List<BaseCodeModel> Children { get; set; }
    }
}
