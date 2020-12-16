using System;
using System.Collections.Generic;
using System.Text;

namespace UMLGenerator.Interfaces
{
    public interface IUMLTransferable
    {
        public string TransferToUML(int layer, Dictionary<string, List<string>> classesDict, Dictionary<string, List<string>> interfacesDict);

    }
}
