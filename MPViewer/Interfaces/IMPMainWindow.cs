using System;

namespace MPViewer.Interfaces
{
    public interface IMPMainWindow : IDisposable
    {
        IMPMainViewModel Model { get; set; }
    }
}