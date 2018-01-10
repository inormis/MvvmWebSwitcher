using System;
using System.Linq;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text.Editor;
using WebSwitcher.Configurations;
using WebSwitcher.IconFiles;

namespace WebSwitcher {
    internal sealed class ViewPortSwitcher {
        /// <summary>
        ///     Distance from the viewport right to the right end of the square box.
        /// </summary>
        private const double ViewMargin = 30;

        /// <summary>
        ///     The layer for the adornment.
        /// </summary>
        private readonly IAdornmentLayer _adornmentLayer;

        private readonly IConfiguration _configuration;


        private readonly DocumentContext _documentContext;

        /// <summary>
        ///     Text view to add the adornment on.
        /// </summary>
        private readonly IWpfTextView _view;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ViewPortSwitcher" /> class.
        ///     Creates a square image and attaches an event handler to the layout changed event that
        ///     adds the the square in the upper right-hand corner of the TextView via the adornment layer
        /// </summary>
        /// <param name="view">The <see cref="IWpfTextView" /> upon which the adornment will be drawn</param>
        /// <param name="globalContext"></param>
        public ViewPortSwitcher(IWpfTextView view, GlobalContext globalContext) {
            _view = view;
            _configuration = ConfigurationRepository.Instance.Configuration;
            _documentContext = new DocumentContext(view, globalContext);
            _adornmentLayer = view.GetAdornmentLayer("ViewPortSwitcher");
            _view.Closed += OnClosed;
            _view.LayoutChanged += OnLayoutChanged;
            _configuration.Changed += Refresh;
        }

        private void OnClosed(object sender, EventArgs e) {
            _documentContext.Dispose();
            _configuration.Changed -= Refresh;
            _view.LayoutChanged -= OnLayoutChanged;
        }

        private void OnLayoutChanged(object sender, EventArgs e) {
            Refresh();
        }

        private void Refresh() {
            if (!_documentContext.IsEnabled)
                return;

            _adornmentLayer.RemoveAllAdornments();

            const int iconsDistane = 5;
            var shift = SwitchButton.ImageSize + iconsDistane;
            var leftMargin = _view.ViewportRight - ViewMargin - shift * _documentContext.SwitchButtons.Count();
            var top = _configuration.Position == SwitchButtonsPosition.Top
                ? _view.ViewportTop + ViewMargin
                : _view.ViewportBottom - ViewMargin - SwitchButton.ImageSize;

            foreach (var switchButton in _documentContext.SwitchButtons) {
                Canvas.SetLeft(switchButton.Image, leftMargin);
                Canvas.SetTop(switchButton.Image, top);
                _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, switchButton.Image, null);

                Canvas.SetLeft(switchButton.Warning, leftMargin + SwitchButton.ImageSize - switchButton.Warning.Width);
                Canvas.SetTop(switchButton.Warning, top + SwitchButton.ImageSize - switchButton.Warning.Height);
                _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, switchButton.Warning, null);

                leftMargin += shift;
            }
        }
    }
}