using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using MPViewer.Interfaces;

namespace MPViewer
{
    public class MPBootstraper
    {
        private CompositionContainer _container;
        private IMPMainViewModel _mainViewModel;

        public MPBootstraper()
        {
            var aggregateCatalog = new AggregateCatalog();
            string currentPath = Directory.GetCurrentDirectory();
            aggregateCatalog.Catalogs.Add(new DirectoryCatalog(currentPath));
            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(MPBootstraper).Assembly));
            _container = new CompositionContainer(aggregateCatalog);
            var composition = new CompositionBatch();
            composition.AddExportedValue(_container);
            _container.Compose(composition);
            _mainViewModel = _container.GetExportedValue<IMPMainViewModel>();
        }

        public void Exit()
        {
            _mainViewModel?.Dispose();
            _mainViewModel = null;
           
        }

        public void Run()
        {
            _mainViewModel.Initialize();
            (_mainViewModel.View as Window).Closed += OnClosed;
            _mainViewModel.ShowMainWindow();
        }
        

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            (_mainViewModel.View as Window).Closed -= OnClosed;
            _mainViewModel.Dispose();
            _mainViewModel = null;
        }
    }
}