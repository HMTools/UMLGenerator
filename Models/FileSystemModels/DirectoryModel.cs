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


        #region Constructors
        public DirectoryModel()
        {
            Items = new ObservableCollection<FileSystemItemModel>();
            Items.CollectionChanged += OnCollectionChanged;
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "IsChecked")
                {
                    IsChangingCheck = true;
                    foreach (var item in Items)
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
        #endregion

        #region Methods
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
                    oldItem.PropertyChanged -= this.OnItemPropertyChanged;
                }
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsChangingCheck)
            {
                FileSystemItemModel item = sender as FileSystemItemModel;
                if (item != null && e.PropertyName == "IsChecked")
                {
                    bool? checkShouldBe =
                        Items.All(item => item.IsChecked == true) ? true :
                        Items.All(item => item.IsChecked == false) ? false : null;
                    if (IsChecked != checkShouldBe)
                    {
                        IsChecked = checkShouldBe;
                    }
                }
            }
        }
        #endregion
    }
}
