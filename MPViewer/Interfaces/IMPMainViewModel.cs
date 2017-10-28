using System;
using System.Collections.Generic;

namespace MPViewer.Interfaces
{
    public interface IMPMainViewModel : IDisposable
    {
        void Initialize();

        void ShowMainWindow();

        object View { get; }

        void AddFiles(IEnumerable<string> files);
    }
}