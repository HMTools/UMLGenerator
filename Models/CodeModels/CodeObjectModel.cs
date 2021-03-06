﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace UMLGenerator.Models.CodeModels
{
    public class CodeObjectModel : BaseModel
    {
        #region Properties

        public string Name { get; set; }
        public Dictionary<string, string> FieldsFound { get; set; } = new Dictionary<string, string>();
        public CodeComponentTypeModel Type { get; set; }
        public CodeObjectModel Parent { get; set; }
        public ObservableCollection<CodeObjectModel> Children { get; set; } = new ObservableCollection<CodeObjectModel>();
        private bool? isChecked = true;

        public bool? IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; NotifyPropertyChanged(); }
        }

        public bool IsChangingCheck { get; set; } = false;

        #endregion Properties

        #region Constructors

        public CodeObjectModel()
        {
            Children.CollectionChanged += OnCollectionChanged;
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "IsChecked")
                {
                    IsChangingCheck = true;
                    foreach (var item in Children)
                    {
                        if (IsChecked == true && item.IsChecked != true && !item.IsChangingCheck)
                        {
                            item.IsChecked = true;
                        }
                        else if (IsChecked == false && item.IsChecked != false && !item.IsChangingCheck)
                        {
                            item.IsChecked = false;
                        }
                    }
                    IsChangingCheck = false;
                }
            };
        }

        #endregion Constructors

        #region Methods

        public virtual string TransferToUML(int layer)
        {
            if (IsChecked == false)
                return "";

            string tab = layer >= 0 ? String.Concat(Enumerable.Repeat("\t", layer)) : "";
            string output = Type.UMLPattern;

            #region Replace Fields (Pattern: [[[<Field Name>]]])

            output = Regex.Replace(output, @"\[\[@Field\((.+?)\)@\]\]", match =>
            {
                var key = match.Groups[1].Value.Trim();
                if (FieldsFound.ContainsKey(key))
                {
                    return FieldsFound[key];
                }
                return "";
            });

            #endregion Replace Fields (Pattern: [[[<Field Name>]]])

            #region Replace Children Components (Pattern: [[|<ComponentType Name>|]])

            output = Regex.Replace(output, @"\[\[@Component\((.+?)\)@\]\]", match =>
            {
                string ret = "";
                var typeName = match.Groups[1].Value.Trim();
                Children.Where(child => child.Type.Name == typeName).ToList()
                .ForEach(child => ret += $"{child.TransferToUML(layer + 1)}");
                return ret;
            });

            #endregion Replace Children Components (Pattern: [[|<ComponentType Name>|]])

            return tab + output + Environment.NewLine;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CodeObjectModel newItem in e.NewItems)
                {
                    newItem.PropertyChanged += this.OnItemPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (CodeObjectModel oldItem in e.OldItems)
                {
                    oldItem.PropertyChanged -= this.OnItemPropertyChanged;
                }
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsChangingCheck)
            {
                if (sender is CodeObjectModel item && e.PropertyName == "IsChecked")
                {
                    bool? checkShouldBe =
                        Children.All(child => child.IsChecked == true) ? true :
                        Children.All(child => child.IsChecked == false) ? false : null;
                    if (IsChecked != checkShouldBe)
                    {
                        IsChecked = checkShouldBe;
                    }
                }
            }
        }

        #endregion Methods
    }
}