using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMLGenerator.Models.FileSystemModels
{
    public class FileModel
    {
        #region Properties
        public string Name { get; set; }
        public string FullName { get; set; }
        public bool IsChecked { get; set; } = true;
        #endregion
    }
}
