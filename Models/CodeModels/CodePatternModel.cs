﻿namespace UMLGenerator.Models.CodeModels
{
    public class CodePatternModel : BaseModel
    {
        #region Properites

        private string pattern = "";

        public string Pattern
        {
            get { return pattern; }
            set { pattern = value; NotifyPropertyChanged(); }
        }

        #endregion Properites
    }
}