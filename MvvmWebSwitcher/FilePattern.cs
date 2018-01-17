using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;

namespace MvvmWebSwitcher {
    public class FilePattern {
        public FilePattern(string action, string controller, string module, string rootDirectory, string filePath) {
            Action = string.IsNullOrWhiteSpace(action) ? "Index" : action;
            Controller = controller;
            Module = module;
            ModulePath = string.IsNullOrWhiteSpace(module) ? "" : $@"{Module}\";
            RootDirectory = rootDirectory;
            FilePath = filePath;

            var roslynWorkspace = GetRoslynWorkspace();
//            roslynWorkspace.
//            Roslyn.RoslynUtilities roslyn = new Parse.Roslyn.RoslynUtilities(roslynWorkspace);
//
//            SyntaxNode node = roslyn.GetNodeByFilePosition(applicationObject.ActiveDocument.FullName, charOffset);
        }

        public string Action { get; }
        public string Controller { get; }
        public string Module { get; }
        public string RootDirectory { get; }
        public string FilePath { get; }
        public string ModulePath { get; }

        private VisualStudioWorkspace GetRoslynWorkspace() {
            var componentModel = (IComponentModel) Package.GetGlobalService(typeof(SComponentModel));
            return componentModel.GetService<VisualStudioWorkspace>();
        }
    }
}