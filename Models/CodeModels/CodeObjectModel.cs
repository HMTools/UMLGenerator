using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UMLGenerator.Models.CodeModels
{
    public class CodeObjectModel : BaseModel
    {
        #region Properties
        public string Name { get; set; }
        public Dictionary<string, string> FieldsFound { get; set; } = new Dictionary<string, string>();
        public CodeComponentTypeModel Type { get; set; }
        public ObservableCollection<CodeObjectModel> Children { get; set; } = new ObservableCollection<CodeObjectModel>();
        private bool? isChecked = true;

        public bool? IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; NotifyPropertyChanged(); }
        }
        #endregion

        #region Fields
        private bool isChangingCheck = false;
        #endregion

        #region Constructors
        public CodeObjectModel()
        {
            Children.CollectionChanged += OnCollectionChanged;
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "IsChecked")
                {
                    isChangingCheck = true;
                    foreach (var item in Children)
                    {
                        if (IsChecked == true && item.IsChecked != true && !item.isChangingCheck)
                        {
                            item.IsChecked = true;
                        }
                        else if (IsChecked == false && item.IsChecked != false && !item.isChangingCheck)
                        {
                            item.IsChecked = false;
                        }
                    }
                    isChangingCheck = false;
                }
            };
        }
        #endregion

        #region Methods
        public virtual string TransferToUML(int layer)
        {
            string tab = layer >= 0 ?String.Concat(Enumerable.Repeat("\t", layer)) : "";
            string output = Type.UMLPattern;

            #region Replace Fields (Pattern: [[[<Field Name>]]])
            output = Regex.Replace(output, @"\[\[\*(.+?)\*\]\]", match =>
            {
                var key = match.Groups[1].Value.Trim();
                if (FieldsFound.ContainsKey(key))
                {
                    return FieldsFound[key];
                }
                return "";
            });
            #endregion

            #region Replace Children Components (Pattern: [[|<ComponentType Name>|]])
            output = Regex.Replace(output, @"\[\[\|(.+?)\|\]\]", match =>
            {
                string ret = "";
                var typeName = match.Groups[1].Value.Trim();
                Children.Where(child => child.Type.Name == typeName).ToList()
                .ForEach(child => ret += $"{child.TransferToUML(layer + 1)}");
                return ret;
            });
            #endregion
            return tab + output + Environment.NewLine;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CodeObjectModel newItem in e.NewItems)
                {
                    //Add listener for each item on PropertyChanged event
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
            if (!isChangingCheck)
            {
                CodeObjectModel item = sender as CodeObjectModel;
                if (item != null && e.PropertyName == "IsChecked")
                {
                    bool? checkShouldBe;
                    if (Children.Count(child => child.IsChecked == true) == Children.Count)
                    {
                        checkShouldBe = true;
                    }
                    else if (Children.Count(child => child.IsChecked == false) == Children.Count)
                    {
                        checkShouldBe = false;
                    }
                    else
                    {
                        checkShouldBe = null;
                    }
                    if(IsChecked != checkShouldBe)
                    {
                        IsChecked = checkShouldBe;
                    }
                }
            }
        }
        #endregion
    }
}
