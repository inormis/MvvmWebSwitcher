using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Text.Editor;
using WebSwitcher.Configurations;
using WebSwitcher.IconFiles;
using WebSwitcher.VSExtensionTools;

namespace WebSwitcher {
    public class DocumentContext : IDocumentContext {
        private readonly GlobalContext _globalContext;

        private readonly IConfiguration _configuration;

        public DocumentContext(IWpfTextView textView, GlobalContext globalContext) {
            _globalContext = globalContext;
            TextView = textView;
            DocumentPath = textView.GetPath();
            Document = globalContext.GetDocumentFromPath(DocumentPath);
            SwitchButtons = new SwitchButton[] {
                new CSharpSwitchButton(this),
                new CssSwitchButton(this),
                new TSSwitchButton(this),
                new HtmlSwitchButton(this)
            };
            foreach (var codeFile in SwitchButtons) {
                codeFile.SwitchClick += OnSwitchClick;
                codeFile.SwitchClick += OnSwitchClick;
            }

            globalContext.RegisterDcoument(this);
            _configuration = ConfigurationRepository.Instance.Configuration;
            _configuration.Changed += UpdateState;
        }

        public bool IsEnabled { get; private set; }

        public IEnumerable<SwitchButton> SwitchButtons { get; }

        public IWpfTextView TextView { get; }

        public string DocumentPath { get; }

        public SwitchButton CurrentSwitchButton { get; private set; }

        public Document Document { get; }

        private void UpdateState() {
            CurrentSwitchButton = SwitchButtons.FirstOrDefault(x => x.SupportsSwitch(DocumentPath));
            IsEnabled = CurrentSwitchButton != null;
        }

        private void OnSwitchClick(SwitchContext switchContext) {
            _globalContext.Open(switchContext);
        }

        public void Dispose() {
            foreach (var switchButton in SwitchButtons) {
                switchButton.SwitchClick -= OnSwitchClick;
                switchButton.Dispose();
            }

            _configuration.Changed -= UpdateState;
            _globalContext.UnregisterDocument(this);
        }

        public void Activate() {
            if (IsEnabled)
                foreach (var button in SwitchButtons)
                    button.Activate();
        }

        public void Deactivate() {
            foreach (var button in SwitchButtons) button.Deactivate();
        }
    }
}