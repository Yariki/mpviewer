using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using MPViewer.Annotations;

namespace MPViewer.Models
{
    public class MPPhoto : INotifyPropertyChanged
    {
        private BitmapSource thumbnail = null;
        private BitmapSource image = null;


        public string PhotoPath { get; set; }

        public BitmapSource Thumbnail
        {
            get { return thumbnail; }
            set
            {
                thumbnail = value;
                OnPropertyChanged();
            }
        }

        public BitmapSource Image
        {
            get { return image; }
            set
            {
                image = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var temp = PropertyChanged;
            temp?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}