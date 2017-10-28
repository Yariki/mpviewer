using MPViewer.Models;

namespace MPViewer.Interfaces
{
    public interface IMPLoadManager
    {
        void LoadThumbnail(MPPhoto photo);
        void LoadFullSize(MPPhoto photo);
    }
}