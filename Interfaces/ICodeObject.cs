using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMLGenerator.Interfaces
{
    public interface ICodeObject
    {
        public void AssociateChilds(List<object> childs);
    }
}
