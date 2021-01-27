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
        public RelayCommand SwitchLocalOrRemoteCommand { get; private set; }
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
        private bool isLocal;

        public bool IsLocal
        {
            get { return isLocal; }
            set { isLocal = value; NotifyPropertyChanged(); }
        }


        public bool IsLoading { get; set; } = false;
        public string SvgString { get; set; }
        public Bitmap UMLImage { get; set; }
        #endregion
        #region Fields
        private CancellationTokenSource cancellationTokenSource;
        private MainViewModel mainVM;
        private bool isJavaInstalled;
        #endregion

        #region Constructors
        public UMLViewModel(MainViewModel mainVM) : base(300, new GridLength(1, GridUnitType.Star), false)
        {
            this.mainVM = mainVM;
            isJavaInstalled = CheckIsJavaInstalled();
            IsLocal = isJavaInstalled;
        }
        #endregion

        #region Methods
        protected override void AddCommands()
        {
            base.AddCommands();
            SwitchViewCommand = new RelayCommand(o => IsUmlView = !IsUmlView);
            CopyToClipboardCommand = new RelayCommand(o => 
            {
                System.Windows.Clipboard.SetText(PlantUml);
                mainVM.SetStatus("Copy PlantUML To Clipboard | Succeed", System.Windows.Media.Brushes.Blue, 2000);
            });
            SwitchLocalOrRemoteCommand = new RelayCommand(o => IsLocal = !IsLocal, o => isJavaInstalled);
        }
        public async void UpdateUML(string plantUML)
        {
            ResetUMLProperties(plantUML);
            IsLoading = true;
            SetUml();
            IsLoading = false;
        }
        private bool CheckIsJavaInstalled()
        {
            var key32 = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32)
                .OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment");
            var key64 = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64)
                .OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment");

            return key32 != null || key64 != null;
        }
        private void ResetUMLProperties(string plantUML)
        {
            PlantUml = plantUML;
            if (IsLoading)
            {
                cancellationTokenSource.Cancel();
                while (IsLoading) ;
            }
            ImageSource = null;
            Message = "Loading ...";
            cancellationTokenSource = new CancellationTokenSource();
        }
        private void SetUml()
        {
            try
            {
                if(IsLocal)
                {
                    Task.Run(async () => {
                        SvgString = await Libraries.PlantUMLMethods.GetLocalSVG(PlantUml, cancellationTokenSource.Token); 
                    });
                    UMLImage = Libraries.PlantUMLMethods.GetLocalPNG(PlantUml, cancellationTokenSource.Token);
                    ImageSource = UMLImage.BitmapToImageSource(System.Drawing.Imaging.ImageFormat.Png);
                }
                else
                {
                    SvgString =  Libraries.PlantUMLMethods.GetRemoteSVG(PlantUml, cancellationTokenSource.Token).GetAwaiter().GetResult();
                    Task.Run(() =>
                    {
                        var svg = Svg.SvgDocument.FromSvg<Svg.SvgDocument>(SvgString);
                        System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            UMLImage = svg.Draw();
                            ImageSource = UMLImage.BitmapToImageSource(System.Drawing.Imaging.ImageFormat.Png);
                        });
                    });
                }
            }
            catch (Exception exception)
            {
                Message = $"Failed Getting UML | {exception.Message}";
            }
        }
        #endregion
    }
}
