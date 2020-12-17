using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMLGenerator.Interfaces
{
    public interface ICodeHasBases
    {
        public List<string> Bases { get; set; }
    }
}
