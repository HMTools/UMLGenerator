using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMLGenerator.ViewModels;

namespace UMLGenerator.Views
{
    /// <summary>
    /// Interaction logic for SelectedSourceView.xaml
    /// </summary>
    public partial class SelectSourceView : UserControl
    {
        public SelectSourceView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.ShowDialog();
            if(dialog.SelectedPath != "")
            {
                (DataContext as SelectSourceViewModel).TargetPath = dialog.SelectedPath;
            }
        }
    }
}
