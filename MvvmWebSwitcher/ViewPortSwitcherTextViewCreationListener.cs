using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace WebSwitcher {
    /// <summary>
    ///     Establishes an <see cref="IAdornmentLayer" /> to place the adornment on and exports the
    ///     <see cref="IWpfTextViewCreationListener" />
    ///     that instantiates the adornment on the event of a <see cref="IWpfTextView" />'s creation
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class ViewPortSwitcherTextViewCreationListener : IWpfTextViewCreationListener {
        public ViewPortSwitcherTextViewCreationListener() {
            _globalContext = new GlobalContext();
        }

        /// <summary>
        ///     Instantiates a ViewPortSwitcher manager when a textView is created.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView" /> upon which the adornment should be placed</param>
        public void TextViewCreated(IWpfTextView textView) {
            new ViewPortSwitcher(textView, _globalContext);
        }
        // Disable "Field is never assigned to..." and "Field is never used" compiler's warnings. Justification: the field is used by MEF.
#pragma warning disable 649, 169

        /// <summary>
        ///     Defines the adornment layer for the scarlet adornment. This layer is ordered
        ///     after the selection layer in the Z-order
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))] [Name("ViewPortSwitcher")] [Order(After = PredefinedAdornmentLayers.Caret)]
        private AdornmentLayerDefinition _editorAdornmentLayer;

        [Import]
        public ITextDocumentFactoryService textDocumentFactory { get; set; }

        private readonly GlobalContext _globalContext;

#pragma warning restore 649, 169
    }
}