using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMLGenerator.Models.CodeModels;

namespace UMLGenerator.Interfaces
{
    public interface ICodeObject
    {
        public ObservableCollection<BaseCodeModel> Children { get; set; }
    }
}
