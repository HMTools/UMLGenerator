using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMLGenerator.Models.CodeModels
{
    public class CodeCleanupModel : BaseModel
    {
        #region Properties
        public string Name { get; set; }
        public string Pattern { get; set; }
        public bool ReplaceWithNewLine { get; set; }
        public bool RegexSignleLine { get; set; }

        #endregion

        #region Constructors
        public CodeCleanupModel(string name, string pattern, bool replaceWithNewLine, bool regexSignleLine)
        {
            Name = name;
            Pattern = pattern;
            ReplaceWithNewLine = replaceWithNewLine;
            RegexSignleLine = regexSignleLine;
        }
        #endregion
    }
}
