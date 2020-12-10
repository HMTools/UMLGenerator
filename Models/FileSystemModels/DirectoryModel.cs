using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMLGenerator.Models.FileSystemModels
{
    public class DirectoryModel
    {
        #region Properties
        public string Name { get; set; }
        public string FullName { get; set; }
        public List<DirectoryModel> Directories { get; set; }
        public List<FileModel> Files { get; set; }
        #endregion
    }
}
