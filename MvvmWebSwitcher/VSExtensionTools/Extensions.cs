using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace WebSwitcher.VSExtensionTools {
    public static class Extensions {
        public static string GetPath(this IWpfTextView textView) {
            textView.TextBuffer.Properties.TryGetProperty(typeof(IVsTextBuffer), out IVsTextBuffer bufferAdapter);
            var persistFileFormat = bufferAdapter as IPersistFileFormat;

            if (persistFileFormat == null) return null;
            persistFileFormat.GetCurFile(out var filePath, out _);
            return filePath;
        }
    }
}