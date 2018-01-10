using System.Windows.Input;
using WebSwitcher.Configurations.Core;

namespace WebSwitcher.Configurations {
    public class ConfigurationViewModel : ViewModelBase {
        private readonly IConfiguration _configuration;

        private string _cSharp;

        private string _css;

        private bool? _dialogResult;

        private string _html;

        private bool _isTopPosition;

        private string _ts;

        public ConfigurationViewModel() {
            _configuration = ConfigurationRepository.Instance.Configuration;
            Css = _configuration.CssRootPath;
            CSharp = _configuration.CSharpRootPath;
            TS = _configuration.TSRootPath;
            Html = _configuration.HtmlRootPath;
            IsTopPosition = _configuration.Position == SwitchButtonsPosition.Top;

            ApplyCommand = new RelayCommand(OnApply);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public ICommand ApplyCommand { get; }
        public ICommand CancelCommand { get; }

        public bool? DialogResult {
            get => _dialogResult;
            private set {
                _dialogResult = value;
                OnPropertyChanged(nameof(DialogResult));
            }
        }

        public string Css {
            get => _css;
            set {
                _css = value;
                OnPropertyChanged(nameof(Css));
            }
        }

        public string CSharp {
            get => _cSharp;
            set {
                _cSharp = value;
                OnPropertyChanged(nameof(CSharp));
            }
        }

        public string Html {
            get => _html;
            set {
                _html = value;
                OnPropertyChanged(nameof(Html));
            }
        }

        public string TS {
            get => _ts;
            set {
                _ts = value;
                OnPropertyChanged(nameof(TS));
            }
        }

        public bool IsTopPosition {
            get => _isTopPosition;
            set {
                _isTopPosition = value;
                OnPropertyChanged(nameof(IsTopPosition));
                OnPropertyChanged(nameof(IsBottomPosition));
            }
        }

        public bool IsBottomPosition {
            get => !IsTopPosition;
            set {
                IsTopPosition = !value;
                OnPropertyChanged(nameof(IsBottomPosition));
            }
        }

        private void OnCancel() {
            DialogResult = false;
        }

        private void OnApply() {
            var position = IsTopPosition ? SwitchButtonsPosition.Top : SwitchButtonsPosition.Bottom;
            _configuration.Update(CSharp, Html, Css, TS, position);
            DialogResult = true;
        }
    }
}