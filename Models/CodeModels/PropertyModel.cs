using System;
using System.Collections.Generic;
using System.Text;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public class PropertyModel : IUMLTransferable
    {
        #region Properties
        public string Name { get; set; }
        public string Type { get; set; }
        public string AccessModifier { get; set; }
        #endregion

        #region Methods
        public string TransferToUML(int layer)
        {
            string tab = String.Concat(System.Linq.Enumerable.Repeat("\t", layer));
            return $"{tab}{ViewModels.MainViewModel.AccessModifiersDict[AccessModifier]}{Name} : {Type}\n\n";
        }
        #endregion
    }
}
