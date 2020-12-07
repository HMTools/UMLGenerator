using System;
using System.Collections.Generic;
using System.Text;

namespace UMLGenerator.Interfaces
{
    public interface IUMLTransferable
    {
        public string TransferToUML(int layer);
    }
}
