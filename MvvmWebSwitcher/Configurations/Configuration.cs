using System;

namespace WebSwitcher.Configurations {
    public class Configuration : IConfiguration {
        private string _cSharpPath;

        private string _cssPath;

        private string _htmlPath;

        private string _tsPath;

        public Configuration() {
            Position = SwitchButtonsPosition.Bottom;
            TSRootPath = @"wwwroot\ViewModels\";
            CssRootPath = @"wwwroot\css\";
            CSharpRootPath = @"Controllers\";
            HtmlRootPath = @"Views\";
            Position = SwitchButtonsPosition.Bottom;
        }

        public event Action Changed;

        public void Update(string csharpRootPath, string htmlRootPath, string cssRootPath, string tsRootPath, SwitchButtonsPosition position) {
            CssRootPath = cssRootPath;
            CSharpRootPath = csharpRootPath;
            TSRootPath = tsRootPath;
            HtmlRootPath = htmlRootPath;
            Position = position;
            Changed?.Invoke();
        }

        public SwitchButtonsPosition Position { get; set; }

        public string CssRootPath {
            get => _cssPath;
            set => _cssPath = FixPath(value);
        }

        public string CSharpRootPath {
            get => _cSharpPath;
            set => _cSharpPath = FixPath(value);
        }

        public string HtmlRootPath {
            get => _htmlPath;
            set => _htmlPath = FixPath(value);
        }

        public string TSRootPath {
            get => _tsPath;
            set => _tsPath = FixPath(value);
        }

        private static string FixPath(string value) {
            var trimPath = value.Trim('\\', ' ');
            return $@"{trimPath}\";
        }
    }
}