using System;
using System.Collections.Generic;
using System.Text;

namespace UMLGenerator
{
    public static class Extensions
    {
        #region array
        public static bool Contains(this Array arr, object item)
        {
            return Array.IndexOf(arr, item) > -1;
        }
        #endregion
    }
}
