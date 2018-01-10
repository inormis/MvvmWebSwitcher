using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WebSwitcher.Configurations;

namespace WebSwitcher.IconFiles {
    public abstract class SwitchButton : IDisposable {
        public const int ImageSize = 48;

        private const double InactiveOpacity = 0.54;

        private const int ActiveOpacity = 1;

        private readonly IConfiguration _configuration;

        private readonly Lazy<Image> _image;

        private readonly Lazy<Image> _warning;

        protected readonly IDocumentContext Context;

        private Regex _filePathRegex;

        protected SwitchButton(IDocumentContext context, string fileName) {
            _configuration = ConfigurationRepository.Instance.Configuration;
            _filePathRegex = CreateFilePathRegex();
            Context = context;
            _image = new Lazy<Image>(() => CreateImage(fileName));
            _warning = new Lazy<Image>(CreateWarningImage);

            _configuration.Changed += OnConfugrationChanged;
        }

        public Image Warning => _warning.Value;

        public Image Image => _image.Value;

        public void Dispose() {
            if (_image.IsValueCreated) {
                Image.MouseEnter -= OnMouseEnter;
                Image.MouseLeave -= OnMouseLeave;
                Image.MouseDown -= OnMouseDown;
            }

            _configuration.Changed -= OnConfugrationChanged;
        }

        private static Image CreateWarningImage() {
            return new Image {
                Source = CreateImageSource("Warning.png"),
                Width = 16,
                Height = 16
            };
        }

        private Image CreateImage(string fileName) {
            var image = new Image {
                Source = CreateImageSource(fileName),
                Width = ImageSize,
                Height = ImageSize,
                Opacity = InactiveOpacity,
                Cursor = Cursors.Arrow
            };

            image.MouseEnter += OnMouseEnter;
            image.MouseLeave += OnMouseLeave;
            image.MouseDown += OnMouseDown;

            return image;
        }

        private void OnConfugrationChanged() {
            _filePathRegex = CreateFilePathRegex();
            UpdateOpacity();
        }

        private Regex CreateFilePathRegex() {
            var pattern = GetPathPattern(_configuration);
            return new Regex(pattern, RegexOptions.IgnoreCase);
        }

        protected static string ConvertRootForPathPattern(string root) {
            if (string.IsNullOrWhiteSpace(root))
                return "";
            return root.Trim('\\').Replace(@"\", @"\\") + @"\\";
        }

        protected abstract string GetPathPattern(IConfiguration configuration);

        public event Action<SwitchContext> SwitchClick;

        private void OnMouseDown(object sender, MouseButtonEventArgs e) {
            if (Equals(Context.CurrentSwitchButton))
                return;

            var switchContext = GetSwitchContextOrNull();
            if (switchContext == null)
                return;
            SwitchClick?.Invoke(switchContext);
            e.Handled = true;
        }

        private SwitchContext GetSwitchContextOrNull() {
            var pattern = Context.CurrentSwitchButton.TryGetFilePatternOrNull(Context.DocumentPath);
            if (pattern == null)
                return null;

            var filePath = GetFilePathFromPattern(pattern, _configuration);
            return new SwitchContext(filePath, pattern);
        }

        protected abstract string GetFilePathFromPattern(FilePattern pattern, IConfiguration configuration);

        private void OnMouseLeave(object sender, MouseEventArgs e) {
            UpdateOpacity();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e) {
            Image.Opacity = ActiveOpacity;
        }

        private static ImageSource CreateImageSource(string fileName) {
            var uriSource = new Uri($"pack://application:,,,/WebSwitcher;component/Icons/{fileName}");
            var bitmapImage = new BitmapImage(uriSource);
            return bitmapImage;
        }

        private FilePattern TryGetFilePatternOrNull(string filePath) {
            if (string.IsNullOrWhiteSpace(filePath))
                return null;

            var match = _filePathRegex.Match(filePath);
            if (match.Success) {
                var rootDirectory = filePath.Substring(0, match.Index);
                var action = match.Groups["Action"].Value;
                var controller = match.Groups["Controller"].Value;
                var module = match.Groups["Module"].Value;
                return CreateSwitchPattern(filePath, action, controller, module, rootDirectory);
            }

            return null;
        }

        public bool SupportsSwitch(string filePath) {
            return _filePathRegex.IsMatch(filePath);
        }

        protected virtual FilePattern CreateSwitchPattern(string filePath, string action, string controller,
            string module, string rootDirectory) {
            return new FilePattern(action, controller, module, rootDirectory, filePath);
        }

        public virtual void OnOpened(FilePattern switchContextPattern) { }

        public virtual void Activate() {
            UpdateOpacity();
        }

        private void UpdateOpacity() {
            var switchContext = GetSwitchContextOrNull();

            if (File.Exists(switchContext.FilePath)) {
                Warning.Visibility = Visibility.Collapsed;
                Image.ToolTip = null;
            }
            else {
                Warning.Visibility = Visibility.Visible;
                Image.ToolTip = $"File doesn\'t exist: \'{switchContext.FilePath}\'\r\nClick to create file";
            }

            Image.Opacity = Equals(Context.CurrentSwitchButton) ? ActiveOpacity : InactiveOpacity;
        }

        public virtual void Deactivate() { }
    }
}