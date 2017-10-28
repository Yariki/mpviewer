using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using MPViewer.Interfaces;

namespace MPViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export(typeof(IMPMainWindow))]
    public partial class MainWindow : Window , IMPMainWindow
    {
       string[] exts = new string[] {".bmp",".jpg",".jpeg",".ico",".gif",".tiff", ".png"};

        public MainWindow()
        {
            InitializeComponent();
            ImagesListView.Drop += ImagesListViewOnDrop;
            ImagesListView.DragOver += ImagesListViewOnDragOver;
        }

        
        public IMPMainViewModel Model
        {
            get {return DataContext as IMPMainViewModel;}
            set { DataContext = value; }
        }

        private void ImagesListViewOnDrop(object sender, DragEventArgs dragEventArgs)
        {
            var files = dragEventArgs.Data.GetData(System.Windows.DataFormats.FileDrop, false) as string[];
            if (files == null || files.Length == 0)
            {
                return;
            }
            Model.AddFiles(files);
        }

        private void ImagesListViewOnDragOver(object sender, DragEventArgs dragEventArgs)
        {

            if (dragEventArgs.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])dragEventArgs.Data.GetData(DataFormats.FileDrop);

                dragEventArgs.Effects =
                    files.Select(p => exts.Contains(System.IO.Path.GetExtension(p).ToLowerInvariant())).All(b => b)
                        ? DragDropEffects.Copy
                        : DragDropEffects.None;
            }
            else
                dragEventArgs.Effects = DragDropEffects.None;
            dragEventArgs.Handled = true;
        }


        public void Dispose()
        {
            ImagesListView.Drop -= ImagesListViewOnDrop;
            ImagesListView.DragOver -= ImagesListViewOnDragOver;
        }
    }
}
