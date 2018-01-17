using EnvDTE;
using Microsoft.VisualStudio.Text.Editor;
using MvvmWebSwitcher.IconFiles;

namespace MvvmWebSwitcher {
    public interface IDocumentContext {
        string DocumentPath { get; }

        Document Document { get; }

        SwitchButton CurrentSwitchButton { get; }

        IWpfTextView TextView { get; }
    }
}