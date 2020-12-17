using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UMLGenerator.Models.FileSystemModels
{
    public class FileSystemItemModel :INotifyPropertyChanged
    {

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properties
        public string Name { get; set; }
        public string FullName { get; set; }

        private bool? isChecked = false;

        public bool? IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; NotifyPropertyChanged(); }
        }
        #endregion
    }
}
