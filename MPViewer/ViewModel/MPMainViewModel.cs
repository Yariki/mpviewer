using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Microsoft.Expression.Interactivity.Core;
using MPViewer.Annotations;
using MPViewer.Helpers;
using MPViewer.Interfaces;
using MPViewer.Models;

namespace MPViewer.ViewModel
{
    [Export(typeof(IMPMainViewModel))]
    public class MPMainViewModel : IMPMainViewModel, INotifyPropertyChanged
    {


        #region [needs]

        private Visibility listVisibility;
        private Visibility singleVisibility;
        private MPPhoto selectedPhoto;
        private int selectedIndex;
        private BlurEffect blurEffect;


        #endregion

        public MPMainViewModel()
        {

        }

        #region [public properties]

        public object View { get; private set; }

        public ObservableCollection<MPPhoto> Images { get; private set; }

        public MPPhoto SelectedPhoto
        {
            get { return selectedPhoto; }
            set
            {
                selectedPhoto = value;
                OnPropertyChanged();
            }
        }
        
        public ICommand MouseDoubleClickCommand { get; private set; }

        public ICommand KeyDownCommand { get; private set; }

        public ICommand UpCommand { get; private set; }

        public ICommand DownCommand { get; private set; }

        public ICommand BlurCommand { get; private set; }

        public Visibility ListVisibility
        {
            get { return listVisibility; }
            set
            {
                listVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility SingleVisibility
        {
            get { return singleVisibility; }
            set
            {
                singleVisibility = value;
                OnPropertyChanged();
            }
        }

        public BlurEffect BlurEffect
        {
            get { return blurEffect; }
            set
            {
                blurEffect = value; 
                OnPropertyChanged();
            }
        }

        #endregion

        #region [public methods]

        public void Dispose()
        {
            (View as IMPMainWindow).Dispose();
        }

        public void Initialize()
        {
            Images = new ObservableCollection<MPPhoto>();
            MouseDoubleClickCommand = new ActionCommand(MouseDoubleClickCommandExecute);
            KeyDownCommand = new ActionCommand(KeyDownCommandExecuted);
            UpCommand = new MPCommand(UpCommandExecuted,CanUpCommandExcute);
            DownCommand = new MPCommand(DownCommandExecuted,CanDownCommandExcute);
            BlurCommand = new ActionCommand((() => BlurEffect = new BlurEffect() {Radius = 15}));

            ListVisibility = Visibility.Visible;
            SingleVisibility = Visibility.Collapsed;

            View = Container.GetExportedValue<IMPMainWindow>();
            (View as IMPMainWindow).Model = this;
        }

        
        public void ShowMainWindow()
        {
            (View as Window)?.Show();
        }

        public void AddFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var photo = new MPPhoto() {PhotoPath = file};
                Images.Add(photo);
                LoadManager.LoadThumbnail(photo);
            }
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion

        #region [private properties]

        [Import]
        private CompositionContainer Container { get; set; }

        [Import]
        private IMPLoadManager LoadManager { get; set; }


        #endregion

        #region [private methods]

        private void MouseDoubleClickCommandExecute(object param)
        {
            ListVisibility = Visibility.Collapsed;
            SingleVisibility = Visibility.Visible;
            LoadSelectedPhoto();
            if (SelectedPhoto != null)
            {
                selectedIndex = Images.IndexOf(SelectedPhoto);
            }
        }

        private void KeyDownCommandExecuted()
        {
            ListVisibility = Visibility.Visible;
            SingleVisibility = Visibility.Collapsed;
        }

        private void UpCommandExecuted(object arg)
        {
            selectedIndex--;
            SelectedPhoto = Images[selectedIndex];
            BlurEffect = null;
            LoadSelectedPhoto();
        }

        private bool CanUpCommandExcute(object arg)
        {
            return selectedIndex > 0;
        }

        private void DownCommandExecuted(object arg)
        {
            selectedIndex++;
            SelectedPhoto = Images[selectedIndex];
            BlurEffect = null;
            LoadSelectedPhoto();
        }

        private bool CanDownCommandExcute(object arg)
        {
            return selectedIndex < Images.Count - 1;
        }

        private void LoadSelectedPhoto()
        {
            if (SelectedPhoto == null || SelectedPhoto.Image != null)
            {
                return;
            }

            LoadManager.LoadFullSize(SelectedPhoto);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var temp = PropertyChanged;
            temp?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}