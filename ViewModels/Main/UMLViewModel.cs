using MVVMLibrary.ViewModels;
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using UMLGenerator.Interfaces;
using UMLGenerator.Models.CodeModels;
using UMLGenerator.Models.FileSystemModels;
using WPFLibrary.Commands;

namespace UMLGenerator.ViewModels.Main
{
    public class UMLViewModel : BaseGridColumnViewModel
    {
        #region Commands
        public RelayCommand SwitchViewCommand { get; private set; }
        public RelayCommand CopyToClipboardCommand { get; private set; }
        #endregion
        #region Properties
        private string plantUml;

        public string PlantUml
        {
            get { return plantUml; }
            set { plantUml = value; NotifyPropertyChanged(); }
        }

        private bool isUmlView = true;

        public bool IsUmlView
        {
            get { return isUmlView; }
            set { isUmlView = value; NotifyPropertyChanged(); }
        }

        private BitmapImage imageSource;

        public BitmapImage ImageSource
        {
            get { return imageSource; }
            set { imageSource = value; NotifyPropertyChanged(); }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; NotifyPropertyChanged(); }
        }
        public bool IsLoading { get; set; } = false;
        public string SvgString { get; set; }
        public Bitmap UMLImage { get; set; }
        #endregion
        #region Fields
        private CancellationTokenSource cancellationTokenSource;
        private MainViewModel mainVM;
        #endregion

        #region Constructors
        public UMLViewModel(MainViewModel mainVM) : base(300, new GridLength(1, GridUnitType.Star), false)
        {
            this.mainVM = mainVM;
        }
        #endregion

        #region Methods
        protected override void AddCommands()
        {
            base.AddCommands();
            SwitchViewCommand = new RelayCommand(o =>
            {
                IsUmlView = !IsUmlView;
            });
            CopyToClipboardCommand = new RelayCommand(o => 
            {
                System.Windows.Clipboard.SetText(PlantUml);
                mainVM.SetStatus("Copy PlantUML To Clipboard | Succeed", System.Windows.Media.Brushes.Blue, 2000);
            });
        }
        public async void UpdateUML(string plantUml)
        {
            PlantUml = plantUml;
            if (IsLoading)
            {
                cancellationTokenSource.Cancel();
                while (IsLoading) ;
            }
            ImageSource = null;
            Message = "Loading ...";
            cancellationTokenSource = new CancellationTokenSource();
            IsLoading = true;
            try
            {
                SvgString = await Libraries.PlantUMLMethods.GetSVG(plantUml, cancellationTokenSource.Token);
                await Task.Run(() =>
                {
                    var svg = Svg.SvgDocument.FromSvg<Svg.SvgDocument>(SvgString);
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        UMLImage = svg.Draw();
                        ImageSource = UMLImage.BitmapToImageSource(System.Drawing.Imaging.ImageFormat.Png);
                    });
                });
            }
            catch (Exception exception)
            {
                Message = $"Failed Getting UML | {exception.Message}";
            }
            IsLoading = false;
        }
        #endregion
    }
}
