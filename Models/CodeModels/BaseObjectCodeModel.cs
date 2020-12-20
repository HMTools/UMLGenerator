using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public abstract class BaseObjectCodeModel : BaseCodeModel, ICodeObject, ICodeAccessDeclared
    {
        #region Properties
        public string Path { get; set; }
        public string Namespace { get; set; }
        public string AccessModifier { get; set; }
        public ObservableCollection<BaseCodeModel> Children { get; set; }



        #endregion

        #region Fields
        private bool isChangingChecked = false;
        private int checkedNum = 0;
        #endregion

        #region Constructors
        public BaseObjectCodeModel(string statement, string path, string nameSpace) : base(statement)
        {
            Children = new ObservableCollection<BaseCodeModel>();

            Path = path;
            Namespace = nameSpace;

            Children.CollectionChanged += OnCollectionChanged;
            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "IsChecked")
                {
                    isChangingChecked = true;
                    foreach (var item in Children)
                    {
                        if (IsChecked == true && item.IsChecked != true)
                        {
                            checkedNum++;
                            item.IsChecked = true;
                        }
                        else if (IsChecked == false && item.IsChecked != false)
                        {
                            checkedNum--;
                            item.IsChecked = false;
                        }
                    }
                    isChangingChecked = false;
                }
            };
        }
        #endregion

        #region Methods
        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (BaseCodeModel newItem in e.NewItems)
                {
                    //Add listener for each item on PropertyChanged event
                    newItem.PropertyChanged += this.OnItemPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (BaseCodeModel oldItem in e.OldItems)
                {
                    if (oldItem.IsChecked == true)
                        checkedNum--;
                    oldItem.PropertyChanged -= this.OnItemPropertyChanged;
                }
            }
        }

        void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!isChangingChecked)
            {
                BaseCodeModel item = sender as BaseCodeModel;
                if (item != null && e.PropertyName == "IsChecked")
                {
                    checkedNum += item.IsChecked == true ? 1 : -1;
                    if (checkedNum == Children.Count && IsChecked != true)
                    {
                        IsChecked = true;
                    }
                    else if (checkedNum == 0 && IsChecked != false)
                    {
                        IsChecked = false;
                    }
                    else if (checkedNum < Children.Count && checkedNum > 0 && IsChecked != null)
                    {
                        IsChecked = null;
                    }
                }
            }
        }
        #endregion
    }
}
