using System;
using System.Diagnostics;
using System.IO;
using OptimalEducation.Helpers.Webpack.AssetsModel;

namespace OptimalEducation.Helpers.Webpack
{
    //todo write to logs
    public static class WebpackConfigWatcher
    {
        private static FileSystemWatcher _watcher;
        private static WebpackAssets _webpackAssets;
        private static string _fileName;

        public static void Initialize(string path)
        {
            _fileName = path;
            var fileInfo = new FileInfo(path);
            InitializeWatcher(fileInfo);

            _webpackAssets = WebpackConfigParser.GetWebpackAssetsJson(_fileName);
            _previousWriteTime = DateTime.Now;
            Debug.WriteLine($"Initial Webpack assets was read.");
        }

        private static void InitializeWatcher(FileInfo fileInfo)
        {
            _watcher = new FileSystemWatcher
            {
                Path = fileInfo.DirectoryName,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime,
                Filter = $"*{fileInfo.Name}"
            };

            _watcher.Changed += OnChanged;
            _watcher.Created += OnChanged;
            _watcher.Deleted += OnDelete;
            _watcher.Error += OnError;

            Debug.WriteLine("Webpack assets. Start watching changes...");
            _watcher.EnableRaisingEvents = true;
        }

        private static void OnError(object sender, ErrorEventArgs e)
        {
            Debug.WriteLine(e);
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            PreventDoubleChangeFileHandle(() =>
            {
                Debug.WriteLine($"Frontend assets were changed. Changed type: {e.ChangeType}");
                _webpackAssets = WebpackConfigParser.GetWebpackAssetsJson(e.FullPath);
            });
        }

        private static DateTime _previousWriteTime = DateTime.MinValue;
        private static readonly TimeSpan MeasureAccuracy = new TimeSpan(0, 0, 0, 0, 100);
        private static void PreventDoubleChangeFileHandle(Action action)
        {
            var currentWriteTime = File.GetLastWriteTime(_fileName);
            if (currentWriteTime.Subtract(_previousWriteTime) > MeasureAccuracy)
            {
                action();

                _previousWriteTime = currentWriteTime;
            }
        }

        private static void OnDelete(object source, FileSystemEventArgs e)
        {
            Debug.WriteLine("Assets was deleted.");
        }

        public static WebpackAssets Assets => _webpackAssets;
    }
}