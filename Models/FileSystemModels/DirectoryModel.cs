using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMLGenerator.Models.FileSystemModels
{
    public class DirectoryModel : FileSystemItemModel
    {
        #region Properties
        public ObservableCollection<FileSystemItemModel> Items { get; set; }
        #endregion

        #region Fields
        private int checkedNum = 0;
        private bool isChangingChecked = false;
        #endregion

        #region Constructors
        public DirectoryModel()
        {
            Items = new ObservableCollection<FileSystemItemModel>();
            Items.CollectionChanged += OnCollectionChanged;
            this.PropertyChanged += (s, e) =>
            {
                if(e.PropertyName == "IsChecked")
                {
                    isChangingChecked = true;
                    foreach(var item in Items)
                    {
                        if(IsChecked == true && item.IsChecked != true)
                        {
                            checkedNum++;
                            item.IsChecked = true;
                        }
                        else if(IsChecked == false && item.IsChecked != false)
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
                foreach (FileSystemItemModel newItem in e.NewItems)
                {
                    //Add listener for each item on PropertyChanged event
                    newItem.PropertyChanged += this.OnItemPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (FileSystemItemModel oldItem in e.OldItems)
                {
                    if (oldItem.IsChecked == true)
                        checkedNum--;
                    oldItem.PropertyChanged -= this.OnItemPropertyChanged;
                }
            }
        }

        void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(!isChangingChecked)
            {
                FileSystemItemModel item = sender as FileSystemItemModel;
                if (item != null && e.PropertyName == "IsChecked")
                {
                    checkedNum += item.IsChecked == true ? 1 : -1;
                    if (checkedNum == Items.Count && IsChecked != true)
                    {
                        IsChecked = true;
                    }
                    else if (checkedNum == 0 && IsChecked != false)
                    {
                        IsChecked = false;
                    }
                    else if (checkedNum < Items.Count && checkedNum > 0 && IsChecked != null)
                    {
                        IsChecked = null;
                    }
                }
            }
        }
        #endregion
    }
}
