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
        private char openDelimiter;

        public char OpenDelimiter
        {
            get { return openDelimiter; }
            set { openDelimiter = value; NotifyPropertyChanged(); }
        }
        private char closeDelimiter;

        public char CloseDelimiter
        {
            get { return closeDelimiter; }
            set { closeDelimiter = value; NotifyPropertyChanged(); }
        }

        private bool hasClose;

        public bool HasClose
        {
            get { return hasClose; }
            set { hasClose = value; NotifyPropertyChanged(); }
        }
        #endregion
    }
}
