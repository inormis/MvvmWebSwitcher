using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using MvvmWebSwitcher.IconFiles;
using MvvmWebSwitcher.VSExtensionTools;
using Window = EnvDTE.Window;

namespace MvvmWebSwitcher {
    public class GlobalContext {
        private readonly DTE2 _application;

        private readonly List<DocumentContext> _documents = new List<DocumentContext>();

        private readonly DTE _dte;

        private readonly Dictionary<string, IWpfTextView> _pathToTextViewMap =
            new Dictionary<string, IWpfTextView>(StringComparer.InvariantCultureIgnoreCase);

        private DocumentContext _currentDocumentContext;

        public GlobalContext() {
            _dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            _application = Package.GetGlobalService(typeof(SDTE)) as DTE2;
            _application.Events.WindowEvents.WindowActivated += OnWindowActivated;
        }

        public string CurrentDocumentPath { get; private set; }

        public DocumentContext CurrentDocumentContext {
            get => _currentDocumentContext;
            private set {
                _currentDocumentContext?.Deactivate();
                _currentDocumentContext = value;
                _currentDocumentContext?.Activate();
            }
        }

        private void OnWindowActivated(Window gotfocus, Window lostfocus) {
            var activeFilePath = gotfocus.Document?.FullName;
            if (CurrentDocumentPath == activeFilePath)
                return;

            CurrentDocumentPath = activeFilePath;
            CurrentDocumentContext = _documents.FirstOrDefault(x => x.DocumentPath == CurrentDocumentPath);
        }

        public void UnregisterDocument(DocumentContext documentContext) {
            _documents.Remove(documentContext);
        }

        public void RegisterDcoument(DocumentContext documentContext) {
            _documents.Add(documentContext);
        }

        public void Open(SwitchContext switchContext) {
            var pathToOpen = switchContext.FilePath;
            if (File.Exists(pathToOpen)) {
                OpenInternal(switchContext);
                return;
            }

            var relativePath =
                pathToOpen.Substring(Path.GetDirectoryName(ExtensionService.Solution.FullName).Length);

            var messageBoxResult = MessageBox.Show($@"Would you like to add file to the project?
Path: {relativePath}", "Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes) {
                var project = GetProjectForFIle(pathToOpen);
                if (project == null) {
                    MessageBox.Show("Couldn't find a project for this path");
                    return;
                }

                var directory = Path.GetDirectoryName(pathToOpen);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllText(pathToOpen, "");
                project.ProjectItems.AddFromFile(pathToOpen);

                OpenInternal(switchContext);
            }
        }

        private static Project GetProjectForFIle(string filePath) {
            return ExtensionService.GetAllProjects().FirstOrDefault(x => filePath.StartsWith(Path.GetDirectoryName(x.FullName)));
        }

        private void OpenInternal(SwitchContext switchContext) {
            _dte.ItemOperations.OpenFile(switchContext.FilePath);
            CurrentDocumentContext.CurrentSwitchButton?.OnOpened(switchContext.Pattern);
        }

        public Document GetDocumentFromPath(string documentPath) {
            foreach (var document in _dte.Documents.OfType<Document>())
                if (document.FullName == documentPath)
                    return document;

            return null;
        }
    }
}