using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMLGenerator.Models.CodeModels
{
    public class CodeDelimiterModel : BaseModel
    {
        #region Properties
        public char OpenDelimiter { get; set; }
        public char CloseDelimiter { get; set; }
        public bool HasClose { get; set; }
        #endregion
    }
}
