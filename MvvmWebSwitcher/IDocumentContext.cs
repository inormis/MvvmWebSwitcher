using EnvDTE;
using Microsoft.VisualStudio.Text.Editor;
using WebSwitcher.IconFiles;

namespace WebSwitcher {
    public interface IDocumentContext {
        string DocumentPath { get; }

        Document Document { get; }

        SwitchButton CurrentSwitchButton { get; }

        IWpfTextView TextView { get; }
    }
}