using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMLGenerator.Models.FileSystemModels
{
    public class DirectoryModel : FileSystemItemModel
    {
        #region Properties
        public List<FileSystemItemModel> Items { get; set; }
        #endregion
        #region Constructors
        public DirectoryModel()
        {
            Items = new List<FileSystemItemModel>();
        }
        #endregion
    }
}
